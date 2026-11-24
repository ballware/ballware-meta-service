# Architecture

## Project layout

```text
src/Ballware.Meta.Service/          web host: Program, Startup, appsettings, Dockerfile
src/Ballware.Meta.Api/              minimal-API endpoints (meta + service groups), Mapster mappings, bindings
src/Ballware.Meta.Data/             public document types, select-list DTOs, repository contracts
src/Ballware.Meta.Data.Ef/          EF Core repositories, persistables, model
src/Ballware.Meta.Data.Ef.Postgres/ EF Core provider (PostgreSQL), migrations, initialization worker
src/Ballware.Meta.Data.Ef.SqlServer/ EF Core provider (SQL Server)
src/Ballware.Meta.Data.Caching/     cachable repository decorators (entity/lookup/tenant)
src/Ballware.Meta.Caching/          distributed cache abstraction (in-memory or Redis)
src/Ballware.Meta.Jobs/             Quartz background jobs (import, tenant seed)
src/Ballware.Meta.Mcp/              MCP tool implementations
test/                               unit/integration tests
```

Generic editing endpoints and the rights-check plumbing come from the shared package
`ballware.shared.api.endpoints` (source in the `ballware-shared-dotnet` repository, referenced
under `ballware-shared-dotnet/src/...` in citations below).

## Request pipeline

1. `Program.cs` builds the host with configuration order
   `appsettings.json` → `appsettings.{Environment}.json` → `appsettings.local.json` → environment
   variables (`src/Ballware.Meta.Service/Program.cs:11-15`).
2. JSON options force PascalCase on both MVC and minimal-API JSON bodies
   (`src/Ballware.Meta.Service/Startup.cs:52-60, 221-226`).
3. Authentication is JWT bearer: authority/audience from the `Authorization` config section, no
   inbound claim mapping, and a custom `OnMessageReceived` that extracts the bearer token
   (`Startup.cs:156-189`).
4. Two authorization policies are defined (`Startup.cs:191-204`):
   - `metaApi` — a `scope` claim must contain `Authorization.RequiredMetaScope` (default `metaApi`)
   - `serviceApi` — a `scope` claim must contain `Authorization.RequiredServiceScope` (default `serviceApi`)
5. Endpoints are mapped per resource (`Startup.cs:426-466`, see table below). Generic editing
   endpoints come from `ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:22-132`.
6. On exceptions, the pipeline returns `application/problem+json` with a `ProblemDetails` body
   (`Startup.cs:394-419`).
7. CORS applies the `Cors` config section if present (`Startup.cs:206-217`).

## Mounted routes

Each resource is mounted with up to three route groups on the same base path
(`src/Ballware.Meta.Service/Startup.cs:426-466`):

- `MapTenantMetaApi` / `MapTenantServiceApi` style specific operations (Meta API tag, Service API tag)
- `MapTenantableEditingApi<TEntity>(basePath, "meta", "<resource>", "<OperationPrefix>", "<tag>")` —
  the 11 generic CRUD-like operations, group `meta`, rights candidates prefixed `meta.<resource>.`

| Base path | Specific meta tag | Generic tag | Operation prefix | Rights candidate prefix |
| --- | --- | --- | --- | --- |
| `/meta/tenant` | `Tenant` | `Tenant` | `Tenant` | `meta.tenant.*` |
| `/meta/statistic` | `Statistic` | `Statistic` | `Statistic` | `meta.statistic.*` |
| `/meta/entityright` | – (generic only) | `EntityRight` | `EntityRight` | `meta.entityright.*` |
| `/meta/processingstate` | `ProcessingState` | `Processingstate` | generic `Processingstate`, specific `ProcessingState` | `meta.processingstate.*` |
| `/meta/pickvalue` | `Pickvalue` | `Pickvalue` | `Pickvalue` | `meta.pickvalue.*` |
| `/meta/page` | `Page` | `Page` | `Page` | `meta.page.*` |
| `/meta/export` | `Export` (service API only) | `Export` | `Export` | `meta.export.*` |
| `/meta/documentation` | `Documentation` | `Documentation` | `Documentation` | `meta.documentation.*` |
| `/meta/job` | `Job` | `Job` | `Job` | `meta.job.*` |
| `/meta/lookup` | `Lookup` | `Lookup` | `Lookup` | `meta.lookup.*` |
| `/meta/entity` | `Entity` | `Entity` | `Entity` | `meta.entity.*` |

The tag/prefix casing quirk for `processingstate` reflects the mount call
(`Startup.cs:438` passes tag `Processingstate` for the generic group, while
`src/Ballware.Meta.Api/Endpoints/ProcessingStateMetaEndpoint.cs:14` uses `ProcessingState` for the
specific operations); both appear exactly like this in the generated `meta` document.

`WithGroupName` assigns every operation to the `meta` or `service` document; `WithTags` assigns the
resource tag (`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:28-29`,
`src/Ballware.Meta.Api/Endpoints/EntityMetaEndpoint.cs:31-32`).

## Caching

- `Storage.EnableCaching` wraps the entity, lookup, and tenant repositories with cachable decorators
  (`src/Ballware.Meta.Data.Ef.Postgres/ServiceCollectionExtensions.cs:41-91`,
  same pattern for SQL Server in `src/Ballware.Meta.Data.Ef.SqlServer/ServiceCollectionExtensions.cs:39-89`;
  decorators in `src/Ballware.Meta.Data.Caching/`).
- The distributed cache is in-memory when `Cache.RedisConfiguration` is empty, otherwise Redis with
  instance prefix `Cache.RedisInstanceName` (`Startup.cs:141-154`); TTL is
  `Cache.CacheExpirationHours` (`src/Ballware.Meta.Caching/Configuration/CacheOptions.cs:5-8`).

## Background workers

- Quartz is configured from the `Quartz` config section (`Startup.cs:136`) and the Meta job
  registrations come from `AddBallwareMetaBackgroundJobs` (`Startup.cs:322`;
  jobs in `src/Ballware.Meta.Jobs/Internal/`).
- The EF provider registers an `InitializationWorker` hosted service that applies database
  migrations at startup when `Storage.AutoMigrations` is on
  (`src/Ballware.Meta.Data.Ef.Postgres/Internal/InitializationWorker.cs:24-28`). It only runs when
  the host actually starts — generating OpenAPI via `swagger tofile` needs no database
  (see [openapi/README.md](../openapi/README.md)).

## Error contract

Unhandled exceptions are converted to RFC 7807 `ProblemDetails` with content type
`application/problem+json`; in Development the raw exception is included as `detail`
(`Startup.cs:394-419`). Endpoint-level statuses (401/404/400) are described in
[api/generic-crud.md](../api/generic-crud.md).

## CORS

Configured from the `Cors` section (`AllowedOrigins` / `AllowedMethods` / `AllowedHeaders`,
`Startup.cs:206-217`), applied as default policy via `app.UseCors()` (`Startup.cs:421`).

## MCP

A Model Context Protocol endpoint is mounted at `/meta/mcp` when the `Mcp` section is present and
`Mcp.Enabled` is true (`Startup.cs:474-478`; gating in
`ballware-shared-dotnet/src/Ballware.Shared.Mcp.Endpoints/McpEndpoint.cs:13`). See
[concepts/mcp-endpoint.md](mcp-endpoint.md); MCP is not part of the OpenAPI documents.
