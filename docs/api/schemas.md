# Component schemas

Field tables for every component schema of the committed snapshots
[meta.json](../openapi/meta.json) (25 schemas) and [service.json](../openapi/service.json) (24 schemas).
“Nullable” follows the snapshot (`nullable: true` or not). “required” lists fields the OpenAPI body
schema marks as required; response payloads always carry them. UUID fields are GUID strings.

Enum-valued fields:

- `JobStates` → `Unknown | Queued | InProgress | Finished | Error` (`src/Ballware.Meta.Data/Common/JobStates.cs:6-13`)
- `EntityCustomFunctionTypes` → `add | edit | default_add | default_view | default_edit | external | export | import` (`src/Ballware.Meta.Data/Public/EntityMetadata.cs:14-32`)

Embedded JSON strings are documented in [concepts/metadata-model.md](../concepts/metadata-model.md).

## meta document

### Documentation

Source: `docs/openapi/meta.json#/components/schemas/Documentation` — src/Ballware.Meta.Data/Public/Documentation.cs:5-11

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Field` | string | yes |  |
| `Content` | string | yes |  |

### DocumentationSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/DocumentationSelectListEntry` — src/Ballware.Meta.Data/SelectLists/DocumentationSelectListEntry.cs:3-7

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Field` | string | yes |  |

### EntityMetadata

Source: `docs/openapi/meta.json#/components/schemas/EntityMetadata` — src/Ballware.Meta.Data/Public/EntityMetadata.cs:132-203

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Meta` | boolean | no |  |
| `GeneratedSchema` | boolean | no |  |
| `KeyColumn` | string | yes |  |
| `NoIdentity` | boolean | no |  |
| `Application` | string | yes |  |
| `Entity` | string | yes |  |
| `DisplayName` | string | yes |  |
| `BaseUrl` | string | yes |  |
| `ItemMappingScript` | string | yes |  |
| `ItemReverseMappingScript` | string | yes |  |
| `ListQuery` | string | yes | embedded JSON array of query entries |
| `ByIdQuery` | string | yes | embedded JSON array of query entries |
| `NewQuery` | string | yes | embedded JSON array of query entries |
| `ScalarValueQuery` | string | yes |  |
| `SaveStatement` | string | yes | embedded JSON array of statements |
| `RemoveStatement` | string | yes |  |
| `RemovePreliminaryCheckScript` | string | yes |  |
| `ListScript` | string | yes |  |
| `RemoveScript` | string | yes |  |
| `ByIdScript` | string | yes |  |
| `BeforeSaveScript` | string | yes |  |
| `SaveScript` | string | yes |  |
| `Lookups` | string | yes | embedded JSON string |
| `Picklists` | string | yes | embedded JSON string |
| `CustomScripts` | string | yes | embedded JSON string |
| `GridLayout` | string | yes | embedded JSON string |
| `EditLayout` | string | yes | embedded JSON string |
| `CustomFunctions` | string | yes | embedded JSON string |
| `StateColumn` | string | yes |  |
| `StateReasonColumn` | string | yes |  |
| `Templates` | string | yes | embedded JSON string |
| `StateAllowedScript` | string | yes |  |
| `Indices` | string | yes | embedded JSON string |
| `ProviderModelDefinition` | string | yes | embedded JSON string |
| `States` | array of ProcessingState | yes |  |
| `Rights` | array of EntityRight | yes |  |
| `Pickvalues` | array of Pickvalue | yes |  |
| `RightsCheckScript` | string | yes | computed from `CustomScripts.extendedRightsCheck`, read-only |

### EntityRight

Source: `docs/openapi/meta.json#/components/schemas/EntityRight` — src/Ballware.Meta.Data/Public/EntityRight.cs:5-12

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Identifier` | string | yes |  |
| `DisplayName` | string | yes |  |
| `Container` | string | yes |  |

### EntityRightSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/EntityRightSelectListEntry` — src/Ballware.Meta.Data/SelectLists/EntityRightSelectListEntry.cs:3-9

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Identifier` | string | yes | required |
| `Name` | string | yes |  |
| `Container` | string | yes |  |

### EntitySelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/EntitySelectListEntry` — src/Ballware.Meta.Data/SelectLists/EntitySelectListEntry.cs:3-8

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Application` | string | yes | required |
| `Entity` | string | yes | required |
| `Name` | string | yes |  |

### Export

Source: `docs/openapi/meta.json#/components/schemas/Export` — src/Ballware.Meta.Data/Public/Export.cs:5-13

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Application` | string | yes |  |
| `Entity` | string | yes |  |
| `Query` | string | yes | export query identifier |
| `ExpirationStamp` | date-time | yes | expiration of download links (1-day default via `exporturl`) |
| `MediaType` | string | yes | media type of export body/file |

### ExportUrlResult

Source: `docs/openapi/meta.json#/components/schemas/ExportUrlResult` — src/Ballware.Meta.Api/Public/ExportUrlResult.cs:3-6

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `TenantId` | uuid | no |  |
| `Id` | uuid | no |  |

### Job

Source: `docs/openapi/meta.json#/components/schemas/Job` — src/Ballware.Meta.Data/Public/Job.cs:6-20

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Scheduler` | string | yes |  |
| `Identifier` | string | yes |  |
| `Owner` | uuid | yes |  |
| `Options` | string | yes |  |
| `Result` | string | yes |  |
| `State` | JobStates | no | enum, serialized as string |

### JobStates

Source: `docs/openapi/meta.json#/components/schemas/JobStates` — src/Ballware.Meta.Data/Common/JobStates.cs:6-13

Enum (string): `Unknown` | `Queued` | `InProgress` | `Finished` | `Error`

### ``KeyValuePair`2``

Source: ``docs/openapi/meta.json#/components/schemas/KeyValuePair`2`` — shared binding type (System.Collections.Generic.KeyValuePair<string, StringValues>) used by `exporturl` form bodies

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Key` | string | yes | form field name |
| `Value` | array of string | no | form field value(s) |

### Lookup

Source: `docs/openapi/meta.json#/components/schemas/Lookup` — src/Ballware.Meta.Data/Public/Lookup.cs:5-17

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Meta` | boolean | no |  |
| `HasParam` | boolean | no |  |
| `Type` | integer | no |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |
| `ListQuery` | string | yes |  |
| `ByIdQuery` | string | yes |  |

### LookupSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/LookupSelectListEntry` — src/Ballware.Meta.Data/SelectLists/LookupSelectListEntry.cs:3-8

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Identifier` | string | yes | required |
| `Name` | string | yes |  |
| `HasParam` | boolean | no |  |

### MetaEntity

Source: `docs/openapi/meta.json#/components/schemas/MetaEntity` — src/Ballware.Meta.Api/Public/MetaEntity.cs:3-27

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Meta` | boolean | no |  |
| `Application` | string | yes |  |
| `Entity` | string | yes |  |
| `DisplayName` | string | yes |  |
| `BaseUrl` | string | yes |  |
| `ItemMappingScript` | string | yes |  |
| `ItemReverseMappingScript` | string | yes |  |
| `KeyColumn` | string | yes |  |
| `Lookups` | string | yes |  |
| `Picklists` | string | yes |  |
| `CustomScripts` | string | yes |  |
| `GridLayout` | string | yes |  |
| `EditLayout` | string | yes |  |
| `CustomFunctions` | string | yes |  |
| `StateColumn` | string | yes |  |
| `StateReasonColumn` | string | yes |  |
| `Templates` | string | yes |  |

### MetaTenant

Source: `docs/openapi/meta.json#/components/schemas/MetaTenant` — src/Ballware.Meta.Api/Public/MetaTenant.cs:3-9

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `Navigation` | string | yes |  |
| `RightsCheckScript` | string | yes |  |
| `Templates` | string | yes |  |

### Page

Source: `docs/openapi/meta.json#/components/schemas/Page` — src/Ballware.Meta.Data/Public/Page.cs:6-27

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |
| `Layout` | string | yes |  |
| `Lookups` | string | yes |  |
| `Picklists` | string | yes |  |
| `CustomScripts` | string | yes |  |

### PageSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/PageSelectListEntry` — src/Ballware.Meta.Data/SelectLists/PageSelectListEntry.cs:3-7

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |

### Pickvalue

Source: `docs/openapi/meta.json#/components/schemas/Pickvalue` — src/Ballware.Meta.Data/Public/Pickvalue.cs:5-14

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Field` | string | yes |  |
| `Value` | integer | no |  |
| `Text` | string | yes |  |
| `Sorting` | integer | yes |  |

### PickvalueSelectEntry

Source: `docs/openapi/meta.json#/components/schemas/PickvalueSelectEntry` — src/Ballware.Meta.Data/SelectLists/PickvalueSelectEntry.cs:5-9

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `Value` | integer | no |  |

### ProcessingState

Source: `docs/openapi/meta.json#/components/schemas/ProcessingState` — src/Ballware.Meta.Data/Public/ProcessingState.cs:5-15

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `State` | integer | no |  |
| `Name` | string | yes |  |
| `Successors` | string | yes |  |
| `RecordFinished` | boolean | no |  |
| `RecordLocked` | boolean | no |  |
| `ReasonRequired` | boolean | no |  |

### ProcessingStateSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/ProcessingStateSelectListEntry` — src/Ballware.Meta.Data/SelectLists/ProcessingStateSelectListEntry.cs:5-15

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `State` | integer | no |  |
| `Locked` | boolean | no |  |
| `Finished` | boolean | no |  |
| `ReasonRequired` | boolean | no |  |

### Statistic

Source: `docs/openapi/meta.json#/components/schemas/Statistic` — src/Ballware.Meta.Data/Public/Statistic.cs:5-19

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |
| `MappingScript` | string | yes |  |
| `CustomScripts` | string | yes |  |
| `FetchSql` | string | yes |  |
| `FetchScript` | string | yes |  |
| `Layout` | string | yes |  |
| `Meta` | boolean | no |  |

### StatisticSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/StatisticSelectListEntry` — src/Ballware.Meta.Data/SelectLists/StatisticSelectListEntry.cs:3-7

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Identifier` | string | yes | required |
| `Name` | string | yes |  |

### Tenant

Source: `docs/openapi/meta.json#/components/schemas/Tenant` — src/Ballware.Meta.Data/Public/Tenant.cs:77-126

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `Navigation` | string | yes |  |
| `RightsCheckScript` | string | yes |  |
| `Templates` | string | yes |  |
| `ServerScriptDefinitions` | string | yes |  |
| `ManagedDatabase` | boolean | no |  |
| `Provider` | string | yes |  |
| `Server` | string | yes |  |
| `Catalog` | string | yes |  |
| `Schema` | string | yes |  |
| `User` | string | yes |  |
| `Password` | string | yes |  |
| `ReportSchemaDefinition` | string | yes |  |
| `ProviderModelDefinition` | string | yes |  |
| `Seed` | boolean | no |  |

### TenantSelectListEntry

Source: `docs/openapi/meta.json#/components/schemas/TenantSelectListEntry` — src/Ballware.Meta.Data/SelectLists/TenantSelectListEntry.cs:3-6

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |

## service document

Schemas shared with the meta document (`Export`, `Job`, `JobStates`, `Lookup`, `Statistic`,
`EntitySelectListEntry`, `PickvalueSelectEntry`, `ProcessingStateSelectListEntry`,
`TenantSelectListEntry`) have identical field tables in both snapshots.

### EntityCustomFunctionTypes

Source: `docs/openapi/service.json#/components/schemas/EntityCustomFunctionTypes` — src/Ballware.Meta.Data/Public/EntityMetadata.cs:14-32

Enum (string): `add` | `edit` | `default_add` | `default_view` | `default_edit` | `external` | `export` | `import`

### EntitySelectListEntry

Source: `docs/openapi/service.json#/components/schemas/EntitySelectListEntry` — src/Ballware.Meta.Data/SelectLists/EntitySelectListEntry.cs:3-8

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Application` | string | yes | required |
| `Entity` | string | yes | required |
| `Name` | string | yes |  |

### Export

Source: `docs/openapi/service.json#/components/schemas/Export` — src/Ballware.Meta.Data/Public/Export.cs:5-13

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Application` | string | yes |  |
| `Entity` | string | yes |  |
| `Query` | string | yes |  |
| `ExpirationStamp` | date-time | yes |  |
| `MediaType` | string | yes |  |

### ExportCreatePayload

Source: `docs/openapi/service.json#/components/schemas/ExportCreatePayload` — src/Ballware.Meta.Api/Endpoints/ExportMetaEndpoint.cs:12-23

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Application` | string | yes | required |
| `Entity` | string | yes | required |
| `Query` | string | yes | required |
| `ExpirationStamp` | date-time | yes |  |
| `MediaType` | string | yes | required |

### Job

Source: `docs/openapi/service.json#/components/schemas/Job` — src/Ballware.Meta.Data/Public/Job.cs:6-20

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Scheduler` | string | yes |  |
| `Identifier` | string | yes |  |
| `Owner` | uuid | yes |  |
| `Options` | string | yes |  |
| `Result` | string | yes |  |
| `State` | JobStates | no | enum, serialized as string |

### JobCreatePayload

Source: `docs/openapi/service.json#/components/schemas/JobCreatePayload` — src/Ballware.Meta.Api/Endpoints/JobMetaEndpoint.cs:13-18

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Scheduler` | string | yes | required |
| `Identifier` | string | yes | required |
| `Options` | string | yes | embedded JSON string;  |

### JobStates

Source: `docs/openapi/service.json#/components/schemas/JobStates` — src/Ballware.Meta.Data/Common/JobStates.cs:6-13

Enum (string): `Unknown` | `Queued` | `InProgress` | `Finished` | `Error`

### JobUpdatePayload

Source: `docs/openapi/service.json#/components/schemas/JobUpdatePayload` — src/Ballware.Meta.Api/Endpoints/JobMetaEndpoint.cs:20-25

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `State` | JobStates | no |  |
| `Result` | string | yes | embedded JSON string |

### Lookup

Source: `docs/openapi/service.json#/components/schemas/Lookup` — src/Ballware.Meta.Data/Public/Lookup.cs:5-17

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Meta` | boolean | no |  |
| `HasParam` | boolean | no |  |
| `Type` | integer | no |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |
| `ListQuery` | string | yes |  |
| `ByIdQuery` | string | yes |  |

### PickvalueSelectEntry

Source: `docs/openapi/service.json#/components/schemas/PickvalueSelectEntry` — src/Ballware.Meta.Data/SelectLists/PickvalueSelectEntry.cs:5-9

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `Value` | integer | no |  |

### ProcessingStateSelectListEntry

Source: `docs/openapi/service.json#/components/schemas/ProcessingStateSelectListEntry` — src/Ballware.Meta.Data/SelectLists/ProcessingStateSelectListEntry.cs:5-15

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
| `State` | integer | no |  |
| `Locked` | boolean | no |  |
| `Finished` | boolean | no |  |
| `ReasonRequired` | boolean | no |  |

### ServiceEntity

Source: `docs/openapi/service.json#/components/schemas/ServiceEntity` — src/Ballware.Meta.Api/Public/ServiceEntity.cs:3-33

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Application` | string | yes |  |
| `Entity` | string | yes |  |
| `CustomScripts` | ServiceEntityCustomScripts | no |  |
| `GeneratedSchema` | boolean | no |  |
| `NoIdentity` | boolean | no |  |
| `KeyColumn` | string | yes |  |
| `CustomFunctions` | array of ServiceEntityCustomFunction | yes |  |
| `ListQuery` | array of ServiceEntityQueryEntry | yes | parsed query entry array |
| `ByIdQuery` | array of ServiceEntityQueryEntry | yes | parsed query entry array |
| `NewQuery` | array of ServiceEntityQueryEntry | yes | parsed query entry array |
| `ScalarValueQuery` | string | yes |  |
| `StateColumn` | string | yes |  |
| `SaveStatement` | array of ServiceEntityQueryEntry | yes | parsed query entry array |
| `RemoveStatement` | string | yes |  |
| `RemovePreliminaryCheckScript` | string | yes |  |
| `ListScript` | string | yes |  |
| `RemoveScript` | string | yes |  |
| `ByIdScript` | string | yes |  |
| `BeforeSaveScript` | string | yes |  |
| `SaveScript` | string | yes |  |
| `StateAllowedScript` | string | yes |  |
| `Indices` | string | yes | embedded JSON string |

### ServiceEntityCustomFunction

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityCustomFunction` — src/Ballware.Meta.Api/Public/ServiceEntityCustomFunction.cs:6-16

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `id` | string | yes | required |
| `type` | EntityCustomFunctionTypes | no |  |
| `options` | ServiceEntityCustomFunctionOptions | no |  |

### ServiceEntityCustomFunctionOptions

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityCustomFunctionOptions` — src/Ballware.Meta.Api/Public/ServiceEntityCustomFunction.cs:18-25

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `format` | string | yes |  |
| `delimiter` | string | yes |  |

### ServiceEntityCustomScripts

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityCustomScripts` — src/Ballware.Meta.Api/Public/ServiceEntityCustomScripts.cs:3-5

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `ExtendedRightsCheck` | string | yes |  |

### ServiceEntityQueryEntry

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityQueryEntry` — src/Ballware.Meta.Api/Public/ServiceEntityQueryEntry.cs:29-53

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `identifier` | string | yes | required |
| `query` | string | yes | required |
| `description` | string | yes |  |
| `ai_enabled` | boolean | yes |  |
| `parameters` | array of ServiceEntityQueryParameter | yes |  |
| `result_columns` | array of ServiceEntityQueryResultColumn | yes |  |

### ServiceEntityQueryParameter

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityQueryParameter` — src/Ballware.Meta.Api/Public/ServiceEntityQueryEntry.cs:5-15

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `name` | string | yes | required |
| `type` | string | yes | required |
| `description` | string | yes |  |

### ServiceEntityQueryResultColumn

Source: `docs/openapi/service.json#/components/schemas/ServiceEntityQueryResultColumn` — src/Ballware.Meta.Api/Public/ServiceEntityQueryEntry.cs:17-27

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `name` | string | yes | required |
| `type` | string | yes | required |
| `description` | string | yes |  |

### ServiceTenant

Source: `docs/openapi/service.json#/components/schemas/ServiceTenant` — src/Ballware.Meta.Api/Public/ServiceTenant.cs:3-25

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes | required |
| `RightsCheckScript` | string | yes |  |
| `ServerScriptDefinitions` | string | yes | embedded JSON string |
| `ManagedDatabase` | boolean | no |  |
| `Provider` | string | yes | required |
| `Server` | string | yes |  |
| `Catalog` | string | yes |  |
| `Schema` | string | yes |  |
| `User` | string | yes |  |
| `Password` | string | yes |  |
| `ReportDatasourceDefinitions` | array of ServiceTenantReportDatasourceDefinition | yes |  |

### ServiceTenantReportDatasourceDefinition

Source: `docs/openapi/service.json#/components/schemas/ServiceTenantReportDatasourceDefinition` — src/Ballware.Meta.Api/Public/ServiceTenantReportDatasource.cs:22-28

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Name` | string | yes | required |
| `Provider` | string | yes | required |
| `ConnectionString` | string | yes | required |
| `Tables` | array of ServiceTenantReportDatasourceTable | yes |  |

### ServiceTenantReportDatasourceRelation

Source: `docs/openapi/service.json#/components/schemas/ServiceTenantReportDatasourceRelation` — src/Ballware.Meta.Api/Public/ServiceTenantReportDatasource.cs:14-20

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Name` | string | yes |  |
| `ChildTable` | string | yes |  |
| `MasterColumn` | string | yes |  |
| `ChildColumn` | string | yes |  |

### ServiceTenantReportDatasourceTable

Source: `docs/openapi/service.json#/components/schemas/ServiceTenantReportDatasourceTable` — src/Ballware.Meta.Api/Public/ServiceTenantReportDatasource.cs:3-12

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Name` | string | yes |  |
| `Entity` | string | yes |  |
| `Query` | string | yes |  |
| `Relations` | array of ServiceTenantReportDatasourceRelation | yes |  |

### Statistic

Source: `docs/openapi/service.json#/components/schemas/Statistic` — src/Ballware.Meta.Data/Public/Statistic.cs:5-19

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Entity` | string | yes |  |
| `Identifier` | string | yes |  |
| `Name` | string | yes |  |
| `MappingScript` | string | yes |  |
| `CustomScripts` | string | yes |  |
| `FetchSql` | string | yes |  |
| `FetchScript` | string | yes |  |
| `Layout` | string | yes |  |
| `Meta` | boolean | no |  |

### TenantSelectListEntry

Source: `docs/openapi/service.json#/components/schemas/TenantSelectListEntry` — src/Ballware.Meta.Data/SelectLists/TenantSelectListEntry.cs:3-6

| Field | Type | Nullable | Notes |
| --- | --- | --- | --- |
| `Id` | uuid | no |  |
| `Name` | string | yes |  |
