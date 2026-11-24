# Meta API resources

The `meta` document exposes exactly these resource route trees (from
[docs/openapi/meta.json](../openapi/meta.json), mounted at
`src/Ballware.Meta.Service/Startup.cs:426-466`):

`documentation`, `entity`, `entityright`, `export`, `job`, `lookup`, `page`, `pickvalue`,
`processingstate`, `statistic`, `tenant`.

All of them carry the 11 generic editing operations described in
[generic-crud.md](generic-crud.md). `entityright` and `export` are **generic-CRUD-only** on the Meta
API (`/meta/entityright` has no specific meta operations; `MapExportMetaApi` maps none,
`src/Ballware.Meta.Api/Endpoints/ExportMetaEndpoint.cs:30-38` — the export-specific operations live
in the Service API).

> Metadata **exports** (document dumps) may contain other document types — e.g. `document`,
> `mlmodel`, `notification`, `subscription`, or documents of other applications (`ai`, `identity`).
> Those are **not** part of this API surface unless they appear in the OpenAPI snapshot: the current
> snapshot exposes no `/meta/<other>` routes for them.

## documentation

Source: `src/Ballware.Meta.Api/Endpoints/DocumentationMetaEndpoint.cs`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/documentation/documentationforentity/{entity}` | `entity` | `200`, raw documentation string |
| GET | `/meta/documentation/documentationforentityandfield/{entity}/{field}` | `entity`, `field` | `200`, raw documentation string |
| GET | `/meta/documentation/selectlist` | – | `200`, array of `DocumentationSelectListEntry` |
| GET | `/meta/documentation/selectbyid/{id}` | `id` (path) | `200`, `DocumentationSelectListEntry`; `404` |

## entity

Source: `src/Ballware.Meta.Api/Endpoints/EntityMetaEndpoint.cs:25-72`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/entity/metadataforentity/{identifier}` | `identifier` | `200`, `MetaEntity`; `404` |
| GET | `/meta/entity/selectlist` | – | `200`, array of `EntitySelectListEntry` |
| GET | `/meta/entity/selectbyid/{id}` | `id` (path) | `200`, `EntitySelectListEntry`; `404` |
| GET | `/meta/entity/selectbyidentifier/{identifier}` | `identifier` | `200`, `EntitySelectListEntry`; `404` |
| GET | `/meta/entity/selectlistrights` | – | `200`, array of `EntityRightSelectListEntry` |

`metadataforentity` returns the reduced client-facing projection `MetaEntity`
(`src/Ballware.Meta.Api/Public/MetaEntity.cs:3-27`) — layout/script fields without the data-access
statements. `selectlistrights` lists the entity's `EntityRight` catalog entries as select entries
(`src/Ballware.Meta.Data/SelectLists/EntityRightSelectListEntry.cs`).

## job

Source: `src/Ballware.Meta.Api/Endpoints/JobMetaEndpoint.cs:39-48`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/job/pendingjobsforuser` | – | `200`, array of `Job` |

Pending = jobs of the current user in the current tenant
(`JobMetaEndpoint.cs:81-87`).

## lookup

Source: `src/Ballware.Meta.Api/Endpoints/LookupMetaEndpoint.cs:28-47`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/lookup/selectlist` | – | `200`, array of `LookupSelectListEntry` |
| GET | `/meta/lookup/selectbyid/{id}` | `id` (path) | `200`, `LookupSelectListEntry`; `404` |

## page

Source: `src/Ballware.Meta.Api/Endpoints/PageMetaEndpoint.cs:23-50`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/page/pagedataforidentifier/{identifier}` | `identifier` | `200`, `Page` |
| GET | `/meta/page/selectlist` | – | `200`, array of `PageSelectListEntry` |
| GET | `/meta/page/selectbyid/{id}` | `id` (path) | `200`, `PageSelectListEntry`; `404` |

## pickvalue

Source: `src/Ballware.Meta.Api/Endpoints/PickvalueMetaEndpoint.cs:27-45`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/pickvalue/selectlistforentityandfield/{entity}/{field}` | `entity`, `field` | `200`, array of `PickvalueSelectEntry` |
| GET | `/meta/pickvalue/selectbyvalueforentityandfield/{entity}/{field}/{value}` | `entity`, `field`, `value` (int) | `200`, `PickvalueSelectEntry`; `404` |

## processingstate

Source: `src/Ballware.Meta.Api/Endpoints/ProcessingStateMetaEndpoint.cs:24-71`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/processingstate/selectlist` | – | `200`, array of `ProcessingStateSelectListEntry` |
| GET | `/meta/processingstate/selectbyid/{id}` | `id` (path) | `200`, `ProcessingStateSelectListEntry`; `404` |
| GET | `/meta/processingstate/selectlistforentity/{identifier}` | `identifier` | `200`, array of `ProcessingStateSelectListEntry` |
| GET | `/meta/processingstate/selectbystateforentity/{identifier}/{state}` | `identifier`, `state` (int) | `200`, `ProcessingStateSelectListEntry`; `404` |
| GET | `/meta/processingstate/selectlistallsuccessorsforentityandstate/{identifier}/{state}` | `identifier`, `state` (int) | `200`, array of `ProcessingStateSelectListEntry` |

Successors are the states reachable from `state` per the entity's state catalog — used by clients to
offer legal transitions. Note the two tag spellings in the snapshot: generic operations are tagged
`Processingstate`, these specific ones `ProcessingState` (see
[concepts/architecture.md](../concepts/architecture.md)).

## statistic

Source: `src/Ballware.Meta.Api/Endpoints/StatisticMetaEndpoint.cs:24-52`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/statistic/metadataforidentifier/{identifier}` | `identifier` | `200`, `Statistic`; `404` |
| GET | `/meta/statistic/selectlist` | – | `200`, array of `StatisticSelectListEntry` |
| GET | `/meta/statistic/selectbyid/{id}` | `id` (path) | `200`, `StatisticSelectListEntry`; `404` |

## tenant

Source: `src/Ballware.Meta.Api/Endpoints/TenantMetaEndpoint.cs:40-78`

| Method | Path | Parameters | Success response |
| --- | --- | --- | --- |
| GET | `/meta/tenant/metadatafortenant/{tenantId}` | `tenantId` (path) | `200`, `MetaTenant`; `403` if path tenant ≠ caller tenant; `404` |
| GET | `/meta/tenant/allowed` | – | `200`, array of `TenantSelectListEntry` |
| GET | `/meta/tenant/selectlist` | – | `200`, array of `TenantSelectListEntry` |
| GET | `/meta/tenant/selectbyid/{id}` | `id` (path) | `200`, `TenantSelectListEntry` |

`metadatafortenant` returns the reduced `MetaTenant` projection
(`src/Ballware.Meta.Api/Public/MetaTenant.cs:3-9`) — no database credentials — and enforces the
caller/tenant match (`TenantMetaEndpoint.cs:139-143`). `allowed` lists the tenants the caller may
act on (`TenantMetaEndpoint.cs:179-185`).

## Select-list DTOs

Select-list projections are lightweight entries for pickers/autocomplete:

| DTO | Source |
| --- | --- |
| `EntitySelectListEntry` | `src/Ballware.Meta.Data/SelectLists/EntitySelectListEntry.cs:3-8` |
| `EntityRightSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/EntityRightSelectListEntry.cs:3-8` |
| `DocumentationSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/DocumentationSelectListEntry.cs:3-7` |
| `LookupSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/LookupSelectListEntry.cs:3-8` |
| `PageSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/PageSelectListEntry.cs:3-7` |
| `PickvalueSelectEntry` | `src/Ballware.Meta.Data/SelectLists/PickvalueSelectEntry.cs:5-9` |
| `ProcessingStateSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/ProcessingStateSelectListEntry.cs:5-15` |
| `StatisticSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/StatisticSelectListEntry.cs:3-7` |
| `TenantSelectListEntry` | `src/Ballware.Meta.Data/SelectLists/TenantSelectListEntry.cs:3-6` |

Field tables: [schemas.md](schemas.md).
