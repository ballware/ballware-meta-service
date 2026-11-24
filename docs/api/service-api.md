# Service API

The `service` document (17 operations) is the read/participate surface for **other services**. All
operations require the `serviceApi` scope, and — unlike the Meta API — the target tenant is an
explicit `{tenantId}` path segment; there is no claim-based tenant resolution and no rights-script
evaluation on these routes.

Source: `src/Ballware.Meta.Api/Endpoints/*.cs` (`MapXServiceApi` methods), grouped in
[docs/openapi/service.json](../openapi/service.json).

## Operation overview

| Method | Path | Parameters | Success response | Purpose |
| --- | --- | --- | --- | --- |
| GET | `/meta/entity/selectlistfortenant/{tenantId}` | `tenantId` | `200`, array of `EntitySelectListEntry` | List entities of a tenant |
| GET | `/meta/entity/servicemetadatafortenantbyidentifier/{tenantId}/{identifier}` | `tenantId`, `identifier` | `200`, `ServiceEntity`; `404` | Full entity contract for a service |
| GET | `/meta/export/exportbyidfortenant/{tenantId}/{id}` | `tenantId`, `id` | `200`, `Export`; `404` | Read an export record |
| POST | `/meta/export/createexportfortenantbehalfofuser/{tenantId}/{userId}` | `tenantId`, `userId`; body `ExportCreatePayload` | `200`, uuid (new export id) | Register an export the caller produced |
| POST | `/meta/job/createjobfortenantbehalfofuser/{tenantId}/{userId}` | `tenantId`, `userId`; body `JobCreatePayload` | `200`, `Job` | Create a job record on behalf of a user |
| POST | `/meta/job/updatejobfortenantbehalfofuser/{tenantId}/{userId}` | `tenantId`, `userId`; body `JobUpdatePayload` | `200`, `Job` | Update job state/result |
| GET | `/meta/lookup/lookupmetadatabytenant/{tenantId}` | `tenantId` | `200`, array of `Lookup` | All lookup definitions of a tenant |
| GET | `/meta/lookup/lookupmetadatabytenantandid/{tenantId}/{id}` | `tenantId`, `id` | `200`, `Lookup`; `404` | One lookup by id |
| GET | `/meta/lookup/lookupmetadatabytenantandidentifier/{tenantId}/{identifier}` | `tenantId`, `identifier` | `200`, `Lookup`; `404` | One lookup by identifier |
| GET | `/meta/pickvalue/selectbyvaluefortenantandentityandfield/{tenantId}/{entity}/{field}/{value}` | `tenantId`, `entity`, `field`, `value` (int) | `200`, `PickvalueSelectEntry`; `404` | Resolve one pick value |
| GET | `/meta/processingstate/selectbystatefortenantandentity/{tenantId}/{identifier}/{state}` | `tenantId`, `identifier`, `state` (int) | `200`, `ProcessingStateSelectListEntry`; `404` | Resolve one processing state |
| GET | `/meta/processingstate/selectlistallsuccessorsfortenantandentitybystate/{tenantId}/{identifier}/{state}` | `tenantId`, `identifier`, `state` (int) | `200`, array of `ProcessingStateSelectListEntry` | Legal successors of a state |
| GET | `/meta/statistic/metadatafortenantandidentifier/{tenantId}/{identifier}` | `tenantId`, `identifier` | `200`, `Statistic`; `404` | Statistic definition |
| GET | `/meta/tenant/servicemetadatafortenant/{tenantId}` | `tenantId` | `200`, `ServiceTenant`; `404` | Full tenant contract (including DB fields) |
| GET | `/meta/tenant/reportallowedtenants` | – | `200`, array of `TenantSelectListEntry` | Tenants available for reporting |
| GET | `/meta/tenant/reportmetadatasourcesfortenant/{tenantId}` | `tenantId` | `200`, array of `ServiceTenantReportDatasourceDefinition` | Report datasources (meta + tenant) for a tenant |
| GET | `/meta/tenant/reportlookupmetadatafortenantdatasourceandidentifier/{tenantId}/{datasource}/{identifier}` | `tenantId`, `datasource`, `identifier` | `200`, object (free-form); `404` | Extra lookup metadata for a report datasource table |

`src/Ballware.Meta.Api/Endpoints/TenantMetaEndpoint.cs:88-126`,
`EntityMetaEndpoint.cs:83-102`, `ExportMetaEndpoint.cs:47-67`, `JobMetaEndpoint.cs:58-77`,
`LookupMetaEndpoint.cs:57-86`, `PickvalueMetaEndpoint.cs:56-66`,
`ProcessingStateMetaEndpoint.cs:81-97`, `StatisticMetaEndpoint.cs:63-72`.

## Export payloads

`ExportCreatePayload` (`src/Ballware.Meta.Api/Endpoints/ExportMetaEndpoint.cs:12-23`):

| Field | Type | Notes |
| --- | --- | --- |
| `Application` | string, required | application the export belongs to |
| `Entity` | string, required | entity the export covers |
| `Query` | string, required | export query identifier used |
| `ExpirationStamp` | date-time, optional | when the export/download stops being valid |
| `MediaType` | string, required | media type of the produced file |

The create call persists an `Export` record and returns only its **id** (uuid string,
`ExportMetaEndpoint.cs:77-90`). A report service typically: create → fill file via Storage →
fetch/read record with `exportbyidfortenant`.

## Job payloads

`JobCreatePayload` (`src/Ballware.Meta.Api/Endpoints/JobMetaEndpoint.cs:13-18`):
`{ "Scheduler": "<scheduler>", "Identifier": "<identifier>", "Options": "<json-string>" }` — all
required; `Options` is an embedded JSON string (the job's option payload).

`JobUpdatePayload` (`JobMetaEndpoint.cs:20-25`):
`{ "Id": "<guid>", "State": "InProgress", "Result": "<json-string>" }` — `State` is a `JobStates`
value (`Unknown | Queued | InProgress | Finished | Error`,
`src/Ballware.Meta.Data/Common/JobStates.cs:8-12`).

Both calls act on behalf of a user (`{userId}`) and return the stored `Job` object:

```bash
curl -s -X POST "https://localhost:6001/meta/job/createjobfortenantbehalfofuser/<tenantId>/<userId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN" -H "Content-Type: application/json" \
  -d '{ "Scheduler": "<scheduler>", "Identifier": "<identifier>", "Options": "{\"key\":\"value\"}" }'
```

## ServiceEntity

`ServiceEntity` (`src/Ballware.Meta.Api/Public/ServiceEntity.cs:3-33`) is the **contract view** of
entity metadata for service consumers: embedded JSON strings of the Meta API are parsed into typed
arrays here.

| Field | Type | Notes |
| --- | --- | --- |
| `Id` | uuid | |
| `Application`, `Entity` | string | |
| `CustomScripts` | `ServiceEntityCustomScripts` | only `ExtendedRightsCheck` is projected (`ServiceEntityCustomScripts.cs:3-5`) |
| `GeneratedSchema`, `NoIdentity` | boolean | |
| `KeyColumn` | string | user-defined key column or null |
| `CustomFunctions` | array of `ServiceEntityCustomFunction` | `{ id, type, options }`; `type` ∈ `add\|edit\|default_add\|default_view\|default_edit\|external\|export\|import` (`EntityCustomFunctionTypes`, `src/Ballware.Meta.Data/Public/EntityMetadata.cs:14-32`); `options` = `{ format, delimiter }` (`ServiceEntityCustomFunction.cs:6-25`) |
| `ListQuery`, `ByIdQuery`, `NewQuery`, `SaveStatement` | array of `ServiceEntityQueryEntry` | embedded query arrays, parsed into entries |
| `ScalarValueQuery`, `RemoveStatement` | string | single statement definitions |
| `StateColumn`, `StateAllowedScript`, `RemovePreliminaryCheckScript` | string | state/remove behavior |
| `ListScript`, `RemoveScript`, `ByIdScript`, `BeforeSaveScript`, `SaveScript` | string | script hooks |
| `Indices` | string | index definitions |

`ServiceEntityQueryEntry` (`src/Ballware.Meta.Api/Public/ServiceEntityQueryEntry.cs:29-53`) uses
lowercase keys: `identifier`, `query`, `description`, `ai_enabled`,
`parameters` (array of `{name, type, description}`, `ServiceEntityQueryEntry.cs:5-15`),
`result_columns` (array of `{name, type, description}`, `ServiceEntityQueryEntry.cs:17-27`).

## ServiceTenant and report datasources

`ServiceTenant` (`src/Ballware.Meta.Api/Public/ServiceTenant.cs:3-25`) exposes tenant metadata
**including database connection fields** (`ManagedDatabase`, `Provider`, `Server`, `Catalog`,
`Schema`, `User`, `Password`, `ReportDatasourceDefinitions`) — it is restricted to `serviceApi`
callers, not to browsers.

`ServiceTenantReportDatasourceDefinition`
(`src/Ballware.Meta.Api/Public/ServiceTenantReportDatasource.cs:22-28`):
`{ Name, Provider, ConnectionString, Tables[] }` with tables
`{ Name, Entity, Query, Relations[] }` and relations
`{ Name, ChildTable, MasterColumn, ChildColumn }` (`ServiceTenantReportDatasource.cs:3-20`).

`reportmetadatasourcesfortenant/{tenantId}` synthesizes fixed meta-lookup datasources
(`Meta`, `MetaLookups`, `Pickvalues`, `ProcessingStates`) plus per-entity pickvalue/state tables
(`src/Ballware.Meta.Api/Endpoints/TenantMetaEndpoint.cs:192-297`).
`reportlookupmetadatafortenantdatasourceandidentifier/...` returns a small free-form object with
keys like `lookupType`, `lookupId`, `lookupIdentifier`, `lookupEntity`, `lookupField` depending on
the datasource (`TenantMetaEndpoint.cs:299-352`).

## Select entries

Used in list responses: `EntitySelectListEntry`, `ProcessingStateSelectListEntry`,
`PickvalueSelectEntry`, `TenantSelectListEntry` — sources and field tables in
[schemas.md](schemas.md) and [meta-api-resources.md](meta-api-resources.md).
