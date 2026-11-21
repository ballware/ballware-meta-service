# Codex Project Blueprint

This file describes the repository, project, and service structure of this repository as a template for new services with a similar architecture. It can be used as Codex context when starting a new repository.

## Target Architecture

The repository is structured as a layered .NET service. Domain contracts, API endpoints, data access, provider-specific EF Core implementations, caching, jobs, MCP tools, and the executable service host live in separate projects.

The service exposes tenant-aware metadata through multiple surfaces:

- HTTP `<ManagementApiName>` API for administrative CRUD-like operations.
- HTTP `<ServiceApiName>` API for read-oriented service-to-service access.
- MCP endpoint for tool-based access.
- Background jobs for import, seeding, and recurring processing.

## Placeholders And Naming

When deriving a new repository from this blueprint, use explicit placeholders not only for product and domain names, but also for API surfaces and authorization scopes.

- `<Product>`: product or organization name, here `Ballware`.
- `<Domain>`: functional service or domain name, here `Meta`.
- `<ManagementApiName>`: name of the administrative API surface, here `Meta`.
- `<ServiceApiName>`: name of the internal service API surface, here `Service`.
- `<ManagementScope>`: authorization policy or OAuth scope for the administrative API, here `metaApi`.
- `<ServiceScope>`: authorization policy or OAuth scope for the internal service API, here `serviceApi`.
- `<Resource>`: functional resource, for example `Tenant`, `Entity`, `Page`, or `Lookup`.

These placeholders define the most important names:

- Project name: `<Product>.<Domain>.<Layer>`.
- Endpoint mapping: `Map<Resource><ManagementApiName>Api` and `Map<Resource><ServiceApiName>Api`.
- Swagger/OpenAPI group: `<managementApiGroup>` and `<serviceApiGroup>`, here `meta` and `service`.
- Default authorization scope in endpoints: `<ManagementScope>` and `<ServiceScope>`.
- Configuration values: `Authorization.Required<ManagementApiName>Scope` and `Authorization.Required<ServiceApiName>Scope`.

## Repository Structure

```text
.
|-- README.md
|-- <solution>.sln
|-- GitVersion.yml
|-- src/
|   |-- <Product>.<Domain>.Service/
|   |-- <Product>.<Domain>.Api/
|   |-- <Product>.<Domain>.Data/
|   |-- <Product>.<Domain>.Data.Ef/
|   |-- <Product>.<Domain>.Data.Ef.Postgres/
|   |-- <Product>.<Domain>.Data.Ef.SqlServer/
|   |-- <Product>.<Domain>.Data.Caching/
|   |-- <Product>.<Domain>.Caching/
|   |-- <Product>.<Domain>.Jobs/
|   |-- <Product>.<Domain>.Mcp/
|   `-- <Product>.<Domain>.Service.Client/
`-- test/
    |-- <Product>.<Domain>.Api.Tests/
    |-- <Product>.<Domain>.Data.Ef.Postgres.Tests/
    |-- <Product>.<Domain>.Data.Ef.SqlServer.Tests/
    |-- <Product>.<Domain>.Caching.Tests/
    |-- <Product>.<Domain>.Jobs.Tests/
    `-- <Product>.<Domain>.Mcp.Tests/
```

## Project Responsibilities

### Service Host

Project: `src/<Product>.<Domain>.Service/`

The service host is the only executable web project (`Microsoft.NET.Sdk.Web`). It contains `Program.cs`, `Startup.cs`, `appsettings*.json`, the Dockerfile, and service-specific adapters.

Responsibilities:

- Load configuration in this order: `appsettings.json`, `appsettings.{Environment}.json`, `appsettings.local.json`, environment variables.
- Configure Serilog, Kestrel, JSON options, CORS, and exception handling.
- Register JWT Bearer authentication and scope-based authorization policies.
- Select the database provider from storage configuration.
- Register Redis or in-memory distributed cache.
- Register Mapster configuration and mapper.
- Configure HTTP clients for external services with OAuth2 client credentials.
- Map API, MCP, and Swagger endpoints.

Typical subfolders:

- `Adapter/` for implementations of external dependencies.
- `Configuration/` for options classes with DataAnnotations.
- `Extensions/` for service-specific DI or repository hooks.
- `Mappings/` for Mapster profiles.

### API Layer

Project: `src/<Product>.<Domain>.Api/`

The API layer contains Minimal API endpoint extensions, API-specific DTOs, model bindings, and mapping configuration.

Typical subfolders:

- `Endpoints/` for `Map<Resource><ManagementApiName>Api` and `Map<Resource><ServiceApiName>Api`.
- `Public/` for API contracts.
- `Bindings/` for claims, tenant, or query bindings.
- `Mappings/` for Mapster profiles.

Conventions:

- Endpoints are static classes with `IEndpointRouteBuilder` extensions.
- Each resource has separate mapping methods for the API surfaces, for example `Map<Resource><ManagementApiName>Api` and `Map<Resource><ServiceApiName>Api`.
- Handler methods are `internal static async Task<IResult>`.
- Authorization uses policy names or scopes such as `<ManagementScope>` and `<ServiceScope>`.
- OpenAPI metadata is maintained directly on endpoints: `Produces`, `WithName`, `WithGroupName`, `WithTags`, `WithSummary`.

### Data Contracts

Project: `src/<Product>.<Domain>.Data/`

This project contains functional data contracts and repository abstractions without concrete database dependencies.

Typical subfolders:

- `Public/` for externally used domain or DTO models.
- `Persistables/` for persisted database models.
- `Repository/` for interfaces.
- `SelectLists/` for reduced selection and search models.
- `Common/` for enums and shared constants.
- `Utils/` for small domain-oriented helper classes.

Conventions:

- Repository interfaces end with `MetaRepository`.
- Tenant-aware repositories inherit from the shared tenant-aware repository abstractions.
- Public methods are asynchronous and end with `Async`.
- Nullable reference types are enabled; optional results are modeled as `T?`.

### EF Core Base Layer

Project: `src/<Product>.<Domain>.Data.Ef/`

This project contains provider-independent EF Core base logic, shared repository base classes, mapping, seeding, and storage configuration.

Typical subfolders:

- `Repository/` for abstract repository base classes.
- `Mapping/` for persistable/public mappings.
- `Model/` for shared model customizers.
- `Seeding/` and `seed/` for seed files.
- `Configuration/` for storage options.

Conventions:

- Shared data access logic lives in abstract base classes.
- Provider-specific SQL generation or DbContext configuration belongs in provider projects, not here.
- Seed files are copied as content to build and publish outputs.

### EF Core Providers

Projects:

- `src/<Product>.<Domain>.Data.Ef.Postgres/`
- `src/<Product>.<Domain>.Data.Ef.SqlServer/`

Provider projects contain concrete `DbContext` implementations, connection factories, migrations, model customizers, repository implementations, and DI extensions.

Conventions:

- Provider-specific repositories inherit from the abstract base classes in the EF Core base layer.
- Migrations live in the corresponding provider project.
- Registration uses descriptive extension methods such as `Add<Product><Domain>StorageForPostgres` or `Add<Product><Domain>StorageForSqlServer`.
- Provider tests use Testcontainers.

### Caching

Projects:

- `src/<Product>.<Domain>.Caching/`
- `src/<Product>.<Domain>.Data.Caching/`

The general caching project contains cache abstractions and implementations. The data caching project contains caching repository decorators.

Conventions:

- Caching is tenant-aware.
- The service host decides between Redis and in-memory distributed cache.
- Caching repository implementations encapsulate data access caching and do not change repository interfaces.

### Jobs

Project: `src/<Product>.<Domain>.Jobs/`

Contains Quartz jobs, job registration, import/seeding logic, and adapter interfaces for external file storage.

Conventions:

- Jobs are registered through a ServiceCollection extension.
- File access and external services are abstracted through interfaces.
- Job tests use mocks for external dependencies.

### MCP

Project: `src/<Product>.<Domain>.Mcp/`

Contains MCP tools, MCP-specific public models, and mapping configuration.

Conventions:

- Tools are separated by resource.
- MCP models are reduced, tool-friendly projections of functional data.
- Registration is centralized through extension methods in the service host.

### Service Client

Project: `src/<Product>.<Domain>.Service.Client/`

Contains generated or manually maintained client contracts for external consumers. If OpenAPI generation is used, the generator configuration lives in this project.

## Technology And Package Profile

New repositories of this kind should use the same baseline decisions unless there are functional reasons not to:

- .NET with SDK-style projects.
- A consistent `TargetFramework` across all projects.
- Enable `Nullable` and `ImplicitUsings`.
- ASP.NET Core Minimal APIs for HTTP endpoints.
- EF Core for relational persistence.
- Separate provider projects for PostgreSQL and SQL Server.
- Mapster for object mapping.
- Serilog for structured logging.
- Quartz for background jobs.
- Redis optionally through `IDistributedCache`, with in-memory cache as fallback.
- JWT Bearer authentication with scope-based policies.
- Swagger/OpenAPI with separate document groups for the configured API surfaces.
- NUnit, Moq, and coverlet for tests.
- Testcontainers for database-oriented integration tests.

## Shared Ballware.Shared Components

Before reimplementing base functionality, check whether a suitable component already exists in `Ballware.Shared.*`. This repository uses shared components for recurring cross-cutting concerns such as authorization, repository base classes, generic editing endpoints, and MCP endpoint infrastructure.

### Ballware.Shared.Authorization

Used in API, Data, Jobs, and MCP projects.

Purpose:

- Central principal and claim evaluation through `IPrincipalUtils`.
- Access to tenant ID, user ID, rights, and claims without custom claim parsing.
- Shared authorization metadata interfaces such as `ITenantAuthorizationMetadata` and `IEntityAuthorizationMetadata`.

Do not reimplement:

- Manual tenant or user ID extraction from `ClaimsPrincipal`.
- Repeated claim-name handling across the codebase.
- Custom metadata interfaces for tenant/entity rights when the shared interfaces fit.

Typical usage:

- API handlers receive `IPrincipalUtils` through dependency injection.
- Binding types for user, tenant, and claims delegate to `IPrincipalUtils`.
- Public models implement shared authorization interfaces when they are relevant for rights checks or metadata providers.

### Ballware.Shared.Authorization.Jint

Used in the service host.

Purpose:

- Shared Jint-based rights checking for dynamic authorization rules.
- Registration through `AddBallwareSharedJintRightsChecker()`.

Do not reimplement:

- Custom JavaScript/expression evaluation for rights conditions.
- Parallel rights checkers with different evaluation behavior.

### Ballware.Shared.Data

Used in the Data project and in functional public/persistable models.

Purpose:

- Shared public model interfaces such as `IEditable`.
- Shared persistable base types for recurring technical fields.
- Repository abstractions such as `ITenantableRepository<T>`.
- Hook abstractions such as `IRepositoryHook<TPublic, TPersistable>` and `ITenantableRepositoryHook<TPublic, TPersistable>`.

Do not reimplement:

- Custom CRUD or tenantable repository interfaces for standard operations.
- Custom `IEditable`, base ID, or audit concepts when the shared types are sufficient.
- Custom hook interfaces for before/after-save or repository extension points.

Typical usage:

- Repository interfaces for resources inherit from `ITenantableRepository<TPublic>`.
- Public models implement `IEditable`.
- Service-specific repository hooks are registered against the shared hook interfaces.

### Ballware.Shared.Data.Ef

Used in the EF base project.

Purpose:

- Shared EF Core repository base classes such as `TenantableBaseRepository<TPublic, TPersistable>`.
- Standardized implementation for tenant-aware query, save, and remove operations.
- Extension points through repository hooks.

Do not reimplement:

- Generic EF Core CRUD base classes for tenant-aware resources.
- Recurring save/remove/query pipelines.
- Separate mapping between standard repository contracts and EF context when the base classes fit.

Typical usage:

- Provider-independent base repositories inherit from `TenantableBaseRepository`.
- Only functional extensions such as select lists, child collection merges, or provider-specific SQL generation are implemented in the service.

### Ballware.Shared.Api.Endpoints

Used in the service host.

Purpose:

- Shared API endpoint dependencies.
- Generic tenant-aware editing endpoints, for example through `MapTenantableEditingApi<T>()`.
- Standardized HTTP surface for recurring editing operations.

Do not reimplement:

- Resource-specific standard CRUD endpoints when `MapTenantableEditingApi<T>()` can be used.
- Recurring endpoint registration for save, remove, query, or comparable editing operations.

Typical usage:

- The service host registers shared dependencies with `AddBallwareSharedApiDependencies()`.
- Resources receive the generic editing endpoints in addition to their specific management/service endpoints.

### Ballware.Shared.Mcp And Ballware.Shared.Mcp.Authorization

Used in the MCP project.

Purpose:

- Shared MCP abstractions and tool infrastructure.
- MCP-oriented authorization helpers.
- A consistent pattern for tool registration and execution with user/tenant context.

Do not reimplement:

- Custom MCP base abstractions for service provider access, principal evaluation, or authorization.
- Custom parallel tool registration mechanisms when the shared registry is sufficient.

Typical usage:

- MCP tools are implemented per resource and registered through shared registration mechanisms.
- Tools resolve `IPrincipalUtils` and repositories from the service provider.

### Ballware.Shared.Mcp.Endpoints

Used in the service host.

Purpose:

- Shared ASP.NET Core integration for MCP endpoints.
- MCP tool registration through `AddBallwareMcpTools(...)`.
- OAuth protected resource metadata and user MCP endpoint mapping through `UseBallwareMcpOAuthProtectedResource(...)` and `MapBallwareUserMcpEndpoint(...)`.

Do not reimplement:

- Custom MCP endpoint mapping for auth, protected resource metadata, or user context.
- Custom tool registry in the web host.

Typical usage:

- The service host registers MCP tools centrally.
- The service host maps the MCP endpoint, for example under `/meta/mcp` or a path appropriate for the new service.

## Reuse Rule

For new features:

- First check shared packages and existing service patterns.
- If a resource is tenant-aware, use `ITenantableRepository<T>`, `TenantableBaseRepository<TPublic, TPersistable>`, and `MapTenantableEditingApi<T>()` first.
- If claims, tenant, user, or rights are needed, use `IPrincipalUtils` and shared authorization interfaces.
- If standard editing, MCP hosting, or rights checking is needed, use the existing shared registrations.
- Implement only functional differences in the service itself.

## Configuration

Options classes live in `Configuration/` and are registered with `AddOptionsWithValidateOnStart<T>()`, `Bind(...)`, and `ValidateDataAnnotations()`.

Recommended configuration sections:

- `Authorization` for authority, audience, claims, and required scopes such as `<ManagementScope>` and `<ServiceScope>`.
- `Storage` for provider, migrations, seed paths, and caching switches.
- `Cache` for Redis connection, instance name, and expiration settings.
- `Cors` for allowed origins, methods, and headers.
- `Swagger` for OAuth client and scopes.
- `<ExternalService>Client` for service URL, token endpoint, client ID, client secret, and scopes.
- `Quartz` for scheduler and job store configuration.
- `ConnectionStrings` for service and Quartz databases.

Local secrets belong in `appsettings.local.json` or environment variables, not in versioned default configuration files.

## Testing Structure

Tests live under `test/` and mirror the production projects.

Conventions:

- Test projects use `Microsoft.NET.Sdk`, `IsPackable=false`, and `SonarQubeExclude=true`.
- NUnit is the test framework; `NUnit.Framework` is included as a global using in the project.
- Moq is used for interface-based dependencies.
- Provider integration tests use Testcontainers for PostgreSQL or SQL Server.
- Test appsettings files are copied to the test output with `CopyToOutputDirectory=PreserveNewest`.
- Internals can be exposed to matching test projects through `InternalsVisibleTo`.

## Coding Guidelines

These guidelines are derived from the existing source code and should be followed explicitly in new projects.

### Project Files

- Use SDK-style `.csproj` files.
- Set this in all projects:

```xml
<TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings>
<Nullable>enable</Nullable>
```

- Use `Microsoft.NET.Sdk.Web` for web hosts; use `Microsoft.NET.Sdk` for libraries and tests.
- Mark test projects and web hosts as not packable when no NuGet package should be produced.
- Use `ProjectReference` for internal dependencies instead of NuGet packages within the same solution.
- Keep provider and infrastructure packages in the projects that actually use them.

### Formatting

- Use 4 spaces for indentation.
- Use Allman-style braces: opening braces usually appear on a new line.
- Use one blank line to separate logical blocks.
- Use `var` when the type is clear from the right-hand side.
- Avoid unnecessary comments; comment only non-obvious decisions.
- Keep using directives at the top of the file, followed by the namespace declaration.
- Use file-scoped namespaces.
- Prefer descriptive names over abbreviations.

### C# Conventions

- Classes, interfaces, records, enums, properties, and methods use `PascalCase`.
- Local variables, parameters, and private fields/properties use `camelCase`, or descriptive `PascalCase` for private properties when matching the existing style.
- Interfaces start with `I`.
- Asynchronous methods end with `Async`.
- Use `Task<T>` and `await`; avoid blocking calls such as `.Result` or `.Wait()`.
- Use `Guid` for technical IDs.
- Use `IEnumerable<T>` for returns when no list-specific API is required.
- Use `T?` for optional results and check explicitly for `null`.
- String comparisons with functional meaning use explicit `StringComparison`, for example `StringComparison.InvariantCultureIgnoreCase` or `StringComparison.Ordinal`.

### API Endpoints

- Organize endpoints by resource in static classes.
- Name mapping methods using the pattern `Map<Resource><ManagementApiName>Api` and `Map<Resource><ServiceApiName>Api`.
- Use `basePath` as a parameter so routes can be composed centrally in the service host.
- Use scope parameters with defaults to `<ManagementScope>` or `<ServiceScope>` instead of hardcoding service-specific scope names.
- Maintain OpenAPI metadata directly on the endpoint.
- Return HTTP results through `Results.Ok`, `Results.NotFound`, and similar helpers.
- Resolve tenant and user context through central principal/authorization utilities, not duplicated claim-parsing logic.

### Data Access

- Define repository interfaces in the Data project.
- Implement shared logic in the EF base project.
- Implement provider-specific details in separate provider projects.
- Use LINQ for simple queries and Dapper/SQL only for cases that require explicit SQL generation.
- Keep tenant-related filters consistently inside repository methods.
- Use mapping between persistable and public models instead of exposing database models directly through APIs.

### Dependency Injection

- Register features through descriptive `IServiceCollection` extension methods.
- Register dependencies with the smallest sensible lifetime.
- Use `Scoped` for repositories, adapters, and request-oriented services.
- Use `Singleton` for immutable configuration such as Mapster `TypeAdapterConfig`.
- Integrate external services through typed `HttpClient` registrations.

### Mapping

- Use Mapster for object mapping.
- Keep mapping configuration per project in dedicated profiles or extension methods.
- Register all mapping profiles centrally in the service host.
- Avoid ad-hoc mapping in endpoints except for very small projections.

### Error Handling And Logging

- Use centralized exception handling in the service host.
- Log unexpected exceptions with Serilog.
- Return API errors as `ProblemDetails`.
- Show detailed exception information only in Development.

### Tests

- Write tests for API handlers, mapping, repository behavior, caching, and jobs according to the change.
- Use Testcontainers for real database migrations and repository-oriented integration tests.
- Keep test data and `appsettings` in the corresponding test project.
- Test tenant-related filters and NotFound cases explicitly.

## Procedure For A New Repository

1. Create the solution and folder structure according to this blueprint.
2. Define the shared naming scheme for product and domain.
3. Create the Data project with public/persistable models and repository interfaces.
4. Create the API project with resource-based Minimal API endpoints.
5. Create the EF base project with DbContext interface, base classes, mapping, and seeding.
6. Create provider projects for PostgreSQL and SQL Server including migrations.
7. Create the service host with configuration, DI, auth, Swagger, caching, jobs, and endpoint mapping.
8. Mirror projects with tests and set up provider migration tests early.
9. Document tech stack, quick start, configuration, and API surfaces in the README.
