# API surface and conventions

Two OpenAPI documents are served per deployment, each with its own Swagger UI page and raw JSON:

| Document | Title | Paths | Schemas | Swagger JSON |
| --- | --- | --- | --- | --- |
| `meta` | ballware Meta API | 150 | 25 | `/swagger/meta/swagger.json` |
| `service` | ballware Service API | 17 | 24 | `/swagger/service/swagger.json` |

Titles and version `v1` are defined in `src/Ballware.Meta.Service/Startup.cs:355-365`; the UI wiring
is `Startup.cs:480-497`. Committed snapshots of both documents live in [docs/openapi/](../openapi/).

## Grouping, tags, operation ids

- Each endpoint is assigned a **group** (`meta` or `service`, via `WithGroupName`) and a resource
  **tag** (via `WithTags`): `ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:28-29`
  and the `MapXMetaApi` / `MapXServiceApi` mount calls in `src/Ballware.Meta.Api/Endpoints/`.
- Operation ids follow `<OperationPrefix><Name>` with the per-resource prefix, e.g.
  `EntitySelectListRights`, `TenantReportMetaDatasources`, `JobPendingForUser`
  (`.WithName(apiOperationPrefix + "...")` calls, e.g.
  `src/Ballware.Meta.Api/Endpoints/EntityMetaEndpoint.cs:39-40`).
- Schema names drop the `Dto` suffix from internal DTO class names via `CustomSchemaIds`
  (`Startup.cs:343-353`).

## Payload conventions

- Request and response JSON is **PascalCase** (naming policy disabled,
  `Startup.cs:52-60, 221-226`), except for the service-API entity payloads whose embedded custom
  function/query members keep their authored lowercase keys (`id`, `type`, `options`, `identifier`,
  `query`, `parameters`, `result_columns`, … — see
  `src/Ballware.Meta.Api/Public/ServiceEntityQueryEntry.cs:29-53`).
- Send `Content-Type: application/json` for JSON bodies; `Accept: application/json` is the default
  for reads. `export` returns the raw export body typed by the export definition's media type.
- UUIDs are plain GUID strings; `JobStates` is serialized as a string
  (`Unknown|Queued|InProgress|Finished|Error`, `src/Ballware.Meta.Data/Common/JobStates.cs:5-12`).

## The `identifier` query parameter

Generic editing operations do not use fixed SQL semantics — they resolve a **query definition by
identifier** from the entity's metadata (embedded arrays in `ListQuery`, `ByIdQuery`, `NewQuery`,
`SaveStatement`, …). Conventions:

| Operation family | `identifier` parameter | Notes |
| --- | --- | --- |
| `all`, `query`, `new`, `byid`, `save`, `savebatch`, `import`, `export`, `exporturl` | required query parameter | value must be an identifier defined in the entity's metadata payloads; `primary` is the platform default convention (`TenantableEndpointHandlerFactory.cs:17`) |
| `remove/{id}` | absent | uses `primary` internally (`TenantableEndpointHandlerFactory.cs:159`) |
| `download/{tenantId}/{id}` | absent | file download of a prior `exporturl` result |

For `save`/`savebatch` a non-`primary` identifier changes the required right
(`meta.<entity>.<identifier>` instead of `meta.<entity>.edit`); for `export`/`exporturl` the
identifier *is* the right. Details in [generic-crud.md](generic-crud.md).

Multi-value query parameters on `query` are merged into a single delimited value: repeated
`?tag=a&tag=b` reaches the query definition as `|a|b|`
(`TenantableEndpointHandlerFactory.cs:312-329`).

## Anonymous download

`GET /meta/<resource>/download/{tenantId}/{id}` requires no token
(`TenantantableEditingEndpoint.cs:125-132`). It serves export files created by `exporturl` for 24
hours; expired or missing exports return `404`. See [generic-crud.md](generic-crud.md).

## Obtaining tokens

- Swagger UI (`/swagger`) uses the OIDC button with `Swagger.ClientId` and
  `Swagger.RequiredScopes` (authorization code + PKCE, `Startup.cs:491-494`).
- Service-to-service callers (other ballware services) obtain a client-credentials token carrying the
  `serviceApi` scope from the IdP token endpoint — the same mechanism the Meta Service itself uses
  for its outbound calls (`Startup.cs:229-249`):

  ```bash
  curl -s -X POST <tokenEndpoint> \
    -d 'grant_type=client_credentials&client_id=<clientId>&client_secret=<clientSecret>&scope=openid serviceApi'
  ```

  Meta API calls need a user token carrying the `metaApi` scope (and the `tenant` claim).

## Error responses

500-class failures return `application/problem+json` (`Startup.cs:394-419`); expected per-operation
statuses are tabulated in [concepts/tenancy-and-authorization.md](../concepts/tenancy-and-authorization.md).
