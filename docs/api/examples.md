# Worked examples

Copy-pasteable sequences against a local deployment. All paths and parameter names match the
committed snapshots [openapi/meta.json](../openapi/meta.json) / [openapi/service.json](../openapi/service.json).
Placeholders: `$TOKEN` (user token with `metaApi` scope and `tenant` claim), `$SERVICE_TOKEN`
(client token with `serviceApi` scope), `<tenantId>`, `<userId>`, `<guid>`, `<entity>`, `<guid>`
identifiers of your own data.

## Meta API

### Tenant metadata of the caller

```bash
curl -s "https://localhost:6001/meta/tenant/metadatafortenant/<tenantId>" \
  -H "Authorization: Bearer $TOKEN"
```

Returns `MetaTenant` (`Id`, `Name`, `Navigation`, `RightsCheckScript`, `Templates`). `403` when
`<tenantId>` differs from the caller's `tenant` claim.

### Tenant list and "allowed" tenants

```bash
curl -s "https://localhost:6001/meta/tenant/allowed" -H "Authorization: Bearer $TOKEN"
curl -s "https://localhost:6001/meta/tenant/selectlist" -H "Authorization: Bearer $TOKEN"
```

Array of `TenantSelectListEntry` (`Id`, `Name`).

### Entity metadata read (envelope with embedded JSON strings)

```bash
curl -s "https://localhost:6001/meta/entity/byid?identifier=primary&id=<guid>" \
  -H "Authorization: Bearer $TOKEN"
```

Response (`EntityMetadata`; child collections are arrays, payload fields are embedded strings):

```json
{
  "Id": "<guid>",
  "Application": "<application>",
  "Entity": "<entity>",
  "DisplayName": "<display name>",
  "Meta": false,
  "GeneratedSchema": true,
  "KeyColumn": null,
  "NoIdentity": false,
  "ListQuery": "[{identifier:'primary',query:'select ...'},{identifier:'exportjson',query:'select ...'}]",
  "GridLayout": "[{identifier:'primary',editLayout:'primary'}]",
  "EditLayout": "[{identifier:'primary',colCount:2}]",
  "CustomScripts": "{editorInitialized:'',extendedRightsCheck:'return result;'}",
  "StateColumn": null,
  "States": [],
  "Rights": [ { "Id": "<guid>", "Entity": "<entity>", "Identifier": "<application>.<entity>.view", "DisplayName": "<label>", "Container": null } ],
  "Pickvalues": []
}
```

### New template, then save (embedded layout strings in the body)

```bash
curl -s "https://localhost:6001/meta/entity/new?identifier=primary" \
  -H "Authorization: Bearer $TOKEN"
```

Edit the returned template and post it back:

```bash
curl -s -X POST "https://localhost:6001/meta/entity/save?identifier=primary" \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d @entity.json
```

`entity.json` (excerpt — note the **string** payloads for layout/query fields):

```json
{
  "Id": "<guid>",
  "Application": "<application>",
  "Entity": "<entity>",
  "DisplayName": "<display name>",
  "Meta": false,
  "GeneratedSchema": true,
  "KeyColumn": null,
  "ListQuery": "[{identifier:'primary',query:'select ... where TenantId = @tenantId'},{identifier:'exportjson',query:'select ...'}]",
  "ByIdQuery": "[{identifier:'primary',query:'select ... where Id = @id'}]",
  "NewQuery": "[{identifier:'primary',query:'select ... template ...'}]",
  "SaveStatement": "[{identifier:'primary',query:'UPDATE ...'}]",
  "RemoveStatement": "delete from <entity> where TenantId=@tenantId",
  "GridLayout": "[{identifier:'primary',editLayout:'primary'}]",
  "EditLayout": "[{identifier:'primary',colCount:2}]",
  "CustomFunctions": "[{id:'exportjson',type:'export',options:{format:'json'}}]",
  "States": [],
  "Rights": [],
  "Pickvalues": []
}
```

Saving with `GeneratedSchema: true` and a `ProviderModelDefinition` triggers tenant schema
provisioning (see [concepts/data-integration-and-jobs.md](../concepts/data-integration-and-jobs.md)).

A dedicated save identifier switches the required right — e.g. `?identifier=providermodel`
requires `meta.entity.providermodel`:

```bash
curl -s -X POST "https://localhost:6001/meta/entity/save?identifier=providermodel" \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{ "Id": "<guid>", "Application": "<application>", "Entity": "<entity>", "ProviderModelDefinition": "{\"tableName\":\"<entity>\", ...}" }'
```

### Query with parameters (including multi-value params)

```bash
curl -s "https://localhost:6001/meta/lookup/query?identifier=primary&search=<term>" \
  -H "Authorization: Bearer $TOKEN"

# multi-value: the query definition receives tag = "|alpha|beta|"
curl -s "https://localhost:6001/meta/lookup/query?identifier=primary&tag=alpha&tag=beta" \
  -H "Authorization: Bearer $TOKEN"
```

The additional parameters (`search`, `tag`, …) are passed to the query definition selected by
`identifier`. Any resource supports the same shape (`/meta/<resource>/query`), and extra
identifiers beyond `primary` exist whenever the document defines them.

### Batch save

```bash
curl -s -X POST "https://localhost:6001/meta/pickvalue/savebatch?identifier=primary" \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '[ { "Id": "<guid>", "Entity": "<entity>", "Field": "<field>", "Value": 10, "Text": "<label>", "Sorting": 1 } ]'
```

### Export → exporturl → anonymous download

Inline export (identifier is the right, here the conventional `exportjson`):

```bash
curl -s "https://localhost:6001/meta/lookup/export?identifier=exportjson&entity=<entity>" \
  -H "Authorization: Bearer $TOKEN" -o export.json
```

File export: form fields become the query parameters of the export query:

```bash
curl -s -X POST "https://localhost:6001/meta/lookup/exporturl?identifier=exportjson" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data "entity=<entity>"
```

```json
{ "TenantId": "<tenantId>", "Id": "<guid>" }
```

Download (no token; expires after ~1 day → `404`):

```bash
curl -s "https://localhost:6001/meta/lookup/download/<tenantId>/<guid>" -o export.json
```

### Import (multipart)

```bash
curl -s -i -X POST "https://localhost:6001/meta/lookup/import?identifier=importjson" \
  -H "Authorization: Bearer $TOKEN" \
  -F "files=@lookup-import.json"
```

`201 Created` immediately; the import runs as a background job that checks
`meta.lookup.importjson` per item. Watch job progress with `GET /meta/job/pendingjobsforuser`.

### Entity right catalog

```bash
curl -s "https://localhost:6001/meta/entity/selectlistrights" -H "Authorization: Bearer $TOKEN"
```

Array of `EntityRightSelectListEntry` (`Identifier` values are right names such as
`<application>.<entity>.view` or `<application>.<entity>.<functionId>`).

### Processing state successors

```bash
curl -s "https://localhost:6001/meta/processingstate/selectlistallsuccessorsforentityandstate/<entity>/10" \
  -H "Authorization: Bearer $TOKEN"
```

Array of `ProcessingStateSelectListEntry` — the states legally reachable from state `10`.

## Service API

### Export record create + fetch

```bash
curl -s -X POST "https://localhost:6001/meta/export/createexportfortenantbehalfofuser/<tenantId>/<userId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN" -H "Content-Type: application/json" \
  -d '{ "Application": "<application>", "Entity": "<entity>", "Query": "exportjson", "ExpirationStamp": "2026-01-01T00:00:00Z", "MediaType": "application/json" }'
```

Response: the new export id (bare uuid). Fetch the record:

```bash
curl -s "https://localhost:6001/meta/export/exportbyidfortenant/<tenantId>/<guid>" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

### Job create + update

```bash
curl -s -X POST "https://localhost:6001/meta/job/createjobfortenantbehalfofuser/<tenantId>/<userId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN" -H "Content-Type: application/json" \
  -d '{ "Scheduler": "<scheduler>", "Identifier": "<identifier>", "Options": "{\"param\":\"value\"}" }'
```

```bash
curl -s -X POST "https://localhost:6001/meta/job/updatejobfortenantbehalfofuser/<tenantId>/<userId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN" -H "Content-Type: application/json" \
  -d '{ "Id": "<guid>", "State": "Finished", "Result": "{\"rows\":42}" }'
```

Both return the stored `Job` document.

### All lookup definitions for a tenant

```bash
curl -s "https://localhost:6001/meta/lookup/lookupmetadatabytenant/<tenantId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

Array of `Lookup`; single variants: `/lookup/lookupmetadatabytenantandid/<tenantId>/<guid>` and
`/lookup/lookupmetadatabytenantandidentifier/<tenantId>/<lookupIdentifier>`.

### Pick value by value

```bash
curl -s "https://localhost:6001/meta/pickvalue/selectbyvaluefortenantandentityandfield/<tenantId>/<entity>/<field>/10" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

Returns `PickvalueSelectEntry` (`Id`, `Name`, `Value`).

### Processing state successors for a tenant

```bash
curl -s "https://localhost:6001/meta/processingstate/selectlistallsuccessorsfortenantandentitybystate/<tenantId>/<entity>/10" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

### Tenant report datasources + lookup metadata

```bash
curl -s "https://localhost:6001/meta/tenant/reportmetadatasourcesfortenant/<tenantId>" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

Array of `ServiceTenantReportDatasourceDefinition` — fixed meta datasources (`Meta`, `MetaLookups`,
`Pickvalues`, `ProcessingStates`) plus the tenant's own report datasources. For a table of the
`MetaLookups` datasource:

```bash
curl -s "https://localhost:6001/meta/tenant/reportlookupmetadatafortenantdatasourceandidentifier/<tenantId>/MetaLookups/entityLookup" \
  -H "Authorization: Bearer $SERVICE_TOKEN"
```

```json
{ "lookupType": "metalookup", "lookupIdentifier": "entityLookup" }
```
