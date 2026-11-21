# Metadata model

The Meta Service stores **metadata documents** — one document type per resource, persisted per
tenant. This page describes the payload shapes as they appear in the API (and in the OpenAPI
components); see [api/schemas.md](../api/schemas.md) for the schema-level field tables.

## EntityMetadata

`EntityMetadata` (`src/Ballware.Meta.Data/Public/EntityMetadata.cs:132-203`) describes one business
entity for one tenant: its queries, layouts, scripts, and the child collections embedded under it.
The generated `meta` schema exposes 39 properties — 38 stored fields plus the read-only computed
`RightsCheckScript`.

| Field | Type | Nullable | Description (source line in `EntityMetadata.cs`) |
| --- | --- | --- | --- |
| `Id` | uuid | no | Document id (134) |
| `Meta` | boolean | no | Entity is central platform metadata, persisted in the metadata database instead of a generated tenant table (136) |
| `GeneratedSchema` | boolean | no | Save/remove triggers tenant schema provisioning via the Schema Service (138; see [data-integration-and-jobs.md](data-integration-and-jobs.md)) |
| `KeyColumn` | string | yes | User-defined unique key column used for lookups instead of the identity column (139) |
| `NoIdentity` | boolean | no | Table has no identity column (141) |
| `Application` | string | yes | Application the entity belongs to, e.g. `<application>` (142) |
| `Entity` | string | yes | Entity identifier within the application, e.g. `<entity>` (143) |
| `DisplayName` | string | yes | Human-readable name (144) |
| `BaseUrl` | string | yes | Base URL pattern the client uses to address the entity (145) |
| `ItemMappingScript` | string | yes | JavaScript mapping applied to items returned to clients (146) |
| `ItemReverseMappingScript` | string | yes | JavaScript mapping applied to items coming from clients (147) |
| `ListQuery` | string | yes | Embedded array of query definitions used by `all`/`query` (149) |
| `ByIdQuery` | string | yes | Embedded array of query definitions used by `byid` (151) |
| `NewQuery` | string | yes | Embedded array of query definitions used by `new` (153) |
| `ScalarValueQuery` | string | yes | Query definition returning scalar values (155) |
| `SaveStatement` | string | yes | Embedded array of save statements used by `save`/`savebatch` (157) |
| `RemoveStatement` | string | yes | Statement used by `remove` (159) |
| `RemovePreliminaryCheckScript` | string | yes | JavaScript precondition check before remove; failing checks produce `400` (161) |
| `ListScript` | string | yes | Script executed for list results (163) |
| `RemoveScript` | string | yes | Script executed on remove (165) |
| `ByIdScript` | string | yes | Script executed for single-item results (167) |
| `BeforeSaveScript` | string | yes | Script executed before saving (169) |
| `SaveScript` | string | yes | Script executed after saving (171) |
| `Lookups` | string | yes | Embedded JSON array of lookup references (172) |
| `Picklists` | string | yes | Embedded JSON array of picklist definitions (173) |
| `CustomScripts` | string | yes | Embedded JSON object of named JavaScript hooks (174; keys in `EntityCustomScripts`, 67-122) |
| `GridLayout` | string | yes | Embedded JSON array of grid layout definitions (175) |
| `EditLayout` | string | yes | Embedded JSON array of edit layout definitions (176) |
| `CustomFunctions` | string | yes | Embedded JSON array of custom function definitions (177; shape in `EntityCustomFunction`, 34-47) |
| `StateColumn` | string | yes | Column holding the processing state value (178) |
| `StateReasonColumn` | string | yes | Column holding a state-change reason (179) |
| `Templates` | string | yes | Embedded JSON array of reusable edit layout templates (180) |
| `StateAllowedScript` | string | yes | JavaScript deciding allowed state transitions (182) |
| `Indices` | string | yes | Embedded JSON array of index definitions (184) |
| `ProviderModelDefinition` | string | yes | Embedded JSON model handed to the Schema Service when `GeneratedSchema` is set (185) |
| `States` | array of `ProcessingState` | yes | Child collection of processing states (187) |
| `Rights` | array of `EntityRight` | yes | Child collection of right catalog entries (188) |
| `Pickvalues` | array of `Pickvalue` | yes | Child collection of picklist values (189) |
| `RightsCheckScript` | string | yes | **Computed, read-only**: mirrors `CustomScripts.extendedRightsCheck` (191-202) |

## Other document types

| Type | Source | Notes |
| --- | --- | --- |
| `Tenant` | `src/Ballware.Meta.Data/Public/Tenant.cs:77-126` | Embedded `Navigation`, `RightsCheckScript`, `Templates`, `ServerScriptDefinitions`, `ReportSchemaDefinition`, `ProviderModelDefinition` strings; database connection fields (`ManagedDatabase`, `Provider`, `Server`, `Catalog`, `Schema`, `User`, `Password`, `Seed`) |
| `Page` | `src/Ballware.Meta.Data/Public/Page.cs:6-27` | Embedded `Layout`, `Lookups`, `Picklists`, `CustomScripts` |
| `Lookup` | `src/Ballware.Meta.Data/Public/Lookup.cs:5-17` | `Meta`, `HasParam`, `Type`, `Identifier`, `Name`, `ListQuery`, `ByIdQuery` |
| `Pickvalue` | `src/Ballware.Meta.Data/Public/Pickvalue.cs:5-14` | Enum values per entity field (`Entity`, `Field`, `Value`, `Text`, `Sorting`) |
| `ProcessingState` | `src/Ballware.Meta.Data/Public/ProcessingState.cs:5-15` | State catalog per entity (`State`, `Name`, `Successors`, `RecordFinished`, `RecordLocked`, `ReasonRequired`) |
| `Statistic` | `src/Ballware.Meta.Data/Public/Statistic.cs:5-19` | `Entity`, `Identifier`, `Name`, `MappingScript`, `CustomScripts`, `FetchSql`, `FetchScript`, `Layout`, `Meta` |
| `Documentation` | `src/Ballware.Meta.Data/Public/Documentation.cs:5-11` | Help content keyed by `Entity` + `Field` |
| `Export` | `src/Ballware.Meta.Data/Public/Export.cs:5-13` | Export record: `Application`, `Entity`, `Query`, `ExpirationStamp`, `MediaType` |
| `Job` | `src/Ballware.Meta.Data/Public/Job.cs:6-20` | Background job record: `Scheduler`, `Identifier`, `Owner`, `Options`, `Result`, `State` (`JobStates`, `src/Ballware.Meta.Data/Common/JobStates.cs:8-12`) |
| `EntityRight` | `src/Ballware.Meta.Data/Public/EntityRight.cs:5-12` | Right catalog entry: `Entity`, `Identifier`, `DisplayName`, `Container` |

## Embedded JSON-string convention

Layout/query/script payload fields are **strings containing serialized JSON**, not OpenAPI objects
(`src/Ballware.Meta.Data/Public/EntityMetadata.cs:172-185`). Real API bodies therefore contain
escaped payloads:

```json
{
  "Id": "<guid>",
  "Application": "<application>",
  "Entity": "<entity>",
  "ListQuery": "[{identifier:'primary',query:'select ...'},{identifier:'exportjson',query:'select ...'}]",
  "Lookups": "[{\"identifier\":\"<lookupIdentifier>\",\"id\":\"<guid>\"}]",
  "GridLayout": "[{identifier:'primary',editLayout:'primary'}]"
}
```

Unescaped, the value of `ListQuery` is an array of query entries:

```text
[
  { identifier: 'primary',    query: 'select ... where TenantId = @tenantId' },
  { identifier: 'exportjson', query: 'select ...' }
]
```

Child collections `States`, `Rights`, `Pickvalues` are the exception — they are real JSON arrays of
`ProcessingState` / `EntityRight` / `Pickvalue` objects (`EntityMetadata.cs:187-189`).

Note: embedded payload strings are written in a JSON5-style syntax (unquoted keys, single quotes).
They are parsed by the server/client tooling, not by generic JSON parsers; treat the syntax you see
in saved metadata as an authoring convention, not a promise of strict JSON.

## Flags controlling persistence

- `Meta` — central platform metadata (stored centrally) vs. tenant business entity.
- `GeneratedSchema` — the entity's table/schema is (re)provisioned through the Schema Service on
  save and dropped on remove (`src/Ballware.Meta.Service/Extensions/GenericSchemaEntityRepositoryHook.cs:20-43`).
- `NoIdentity` — table is created without an identity column.
- `KeyColumn` — optional user-defined unique key column; when set, lookups can use it instead of the
  identity key (null by default).

## Observed production conventions (anonymized)

Patterns abstracted from real metadata deployments — illustrative, not guarantees; every concrete
identifier below is a placeholder:

- A metadata **document export** is a JSON array wrapping a single object (e.g. one entity document
  exported as `[ { ...EntityMetadata... } ]`).
- Query payload fields hold **arrays with multiple identifier variants**: besides `primary`, entities
  commonly define semantic identifiers such as `name`, `report`, `exportjson`, `importjson`, `state`,
  `context`, `entity` or tenant-specific variants. The generic `query`/`export`/`save` endpoints then
  accept those identifiers via the `identifier` parameter.
- **Custom function ids map to right names**: a function entry
  `{ id: '<functionId>', type: 'external', options: {...} }` inside `CustomFunctions` corresponds to
  a right `<application>.<entity>.<functionId>` in the entity's `Rights` catalog; the function types
  are `add | edit | default_add | default_view | default_edit | external | export | import`
  (`EntityMetadata.cs:14-32`).
- **State lifecycle**: entities with `StateColumn` + `States` define numeric states with named
  transitions (`Successors` referencing other state ids) and state-scoped rights such as
  `<application>.<entity>.visiblestate.<state>`.
- Built-in platform entities commonly use `Meta: true`, `GeneratedSchema: false`, null `KeyColumn`
  and no `ProviderModelDefinition`; tenant business entities commonly use `Meta: false`,
  `GeneratedSchema: true` with `ProviderModelDefinition`-driven custom columns, and an explicit or
  null `KeyColumn`.
- Embedded arrays may contain `__KEY__` entries. Treat `__KEY__` as a storage/export/metadata-editor
  artifact; it is not part of the API contract unless a schema documents it.
- Not every entity found in a metadata export is a Meta API resource — the OpenAPI snapshot defines
  the `/meta/...` surface (see [api/meta-api-resources.md](../api/meta-api-resources.md)).
