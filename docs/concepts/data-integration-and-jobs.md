# Data integration and jobs

## Tenant-scoped query definitions

Business data is not accessed through fixed tables in the Meta database. Each `EntityMetadata`
document defines the statements the repositories execute **per tenant**
(`src/Ballware.Meta.Data/Public/EntityMetadata.cs:149-184`):

| Field | Consumed by |
| --- | --- |
| `ListQuery` | generic `all` / `query` (`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantableEndpointHandlerFactory.cs:67,80`) |
| `ByIdQuery` | generic `byid`, `remove` (item load) (`TenantableEndpointHandlerFactory.cs:101,159`) |
| `NewQuery` | generic `new` (`TenantableEndpointHandlerFactory.cs:92`) |
| `ScalarValueQuery` | scalar lookups used by scripts/clients |
| `SaveStatement` | generic `save` / `savebatch` (`TenantableEndpointHandlerFactory.cs:126,146`) |
| `RemoveStatement` | generic `remove` (`TenantableEndpointHandlerFactory.cs:172-175`) |
| `Indices` | index provisioning metadata for generated schemas |

Each query field is an embedded array of `{ identifier, query }` entries; the `identifier` query
parameter selects which entry runs. The entry named `primary` is the default the platform relies on
(`TenantableEndpointHandlerFactory.cs:17`).

## Schema provisioning

Entities flagged `GeneratedSchema` keep their tenant tables in sync with the metadata document via
repository hooks registered in `src/Ballware.Meta.Service/Startup.cs:307-308`:

- **On save** — when the save ran under identifier `primary`, `importjson` or `providermodel` and the
  document has `GeneratedSchema` plus a non-empty `ProviderModelDefinition`, the hook calls the Schema
  Service operation `TenantCreateOrUpdateEntitySchemaForTenant` with the serialized model
  (`src/Ballware.Meta.Service/Extensions/GenericSchemaEntityRepositoryHook.cs:20-35`).
- **On remove** — `GeneratedSchema` entities drop their tenant table via
  `TenantDropEntitySchemaForTenant` (`GenericSchemaEntityRepositoryHook.cs:37-43`).
- Tenant documents have the analogous `GenericSchemaTenantRepositoryHook`
  (`src/Ballware.Meta.Service/Extensions/GenericSchemaTenantRepositoryHook.cs`).

## Outbound service clients

The service talks to two other platform services with OAuth2 client credentials
(Duende `AddClientCredentialsTokenManagement`):

- clients configured from the `StorageClient` / `SchemaClient` config sections
  (`src/Ballware.Meta.Service/Startup.cs:229-249`),
- typed HTTP clients `StorageServiceClient` and `GenericSchemaClient` attached to those token
  handlers (`Startup.cs:251-273`). In Debug builds the outgoing clients skip server certificate
  validation; in production real certificates are required.

The Storage Service holds temporary files used by imports and file exports
(`IFileStorageProvider` / `IJobsFileStorageAdapter` adapters, `Startup.cs:305-306`).

## Import flow

`POST /meta/<resource>/import` is intentionally fire-and-forget
(`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantableEndpointHandlerFactory.cs:188-220`):

1. The uploaded file is stored as a temporary file on behalf of the user
   (`TenantableEndpointHandlerFactory.cs:208`).
2. A `Job` metadata record is created with `JobMetadataProvider.CreateJobAsync(tenant, user,
   "document", "import", <jobData>)` — the `JobDataMap` keys are `tenantId`, `userId`, `identifier`,
   `claims`, `file`, `jobId` (`TenantableEndpointHandlerFactory.cs:198-212`).
3. The Quartz job `JobKey.Create("import", <entity>)` is triggered
   (`TenantableEndpointHandlerFactory.cs:214`); the endpoint returns `201 Created` without waiting.
4. `TenantableMetaImportJob` later loads the file, imports item by item, and checks the right
   `meta.<entity>.<identifier>` per item through the tenant rights script
   (`src/Ballware.Meta.Jobs/Internal/TenantableMetaImportJob.cs:66-70`).

## Job lifecycle

Job records move through the `JobStates` values `Unknown`, `Queued`, `InProgress`, `Finished`,
`Error` (`src/Ballware.Meta.Data/Common/JobStates.cs:8-12`; import job state transitions in
`TenantableMetaImportJob.cs:59,73,77`).

Other services participate in this lifecycle through the Service API:

- `POST /meta/job/createjobfortenantbehalfofuser/{tenantId}/{userId}` — creates a job record
  (`src/Ballware.Meta.Api/Endpoints/JobMetaEndpoint.cs:58,89-94`),
- `POST /meta/job/updatejobfortenantbehalfofuser/{tenantId}/{userId}` — updates state/result
  (`JobMetaEndpoint.cs:68,96-101`),
- `GET /meta/job/pendingjobsforuser` (Meta API) — the current user's pending jobs for their tenant
  (`JobMetaEndpoint.cs:39,81-87`).

Payload details: [api/service-api.md](../api/service-api.md).
