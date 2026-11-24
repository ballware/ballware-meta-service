# Generic CRUD operations

Every Meta API resource (`tenant`, `statistic`, `entityright`, `processingstate`, `pickvalue`,
`page`, `export`, `documentation`, `job`, `lookup`, `entity`) is mounted with the same 11 generic
editing operations from
`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:22-132`
(handler implementation in `TenantableEndpointHandlerFactory.cs`), plus resource-specific operations
(see [meta-api-resources.md](meta-api-resources.md)).

All 11 operations require the **`metaApi` endpoint policy**. The endpoint policy only checks the
scope — most handlers then compute a **right candidate** and let the tenant/entity rights scripts
decide (see [concepts/tenancy-and-authorization.md](../concepts/tenancy-and-authorization.md)):
`all`/`query`/`byid` use `view`, `new` uses `add`, `remove/{id}` uses `delete`, `save`/`savebatch`
use `edit` unless the `identifier` parameter is present and not `primary` — then the identifier
itself is the right. `export`/`exporturl` always use the identifier as the right. `import` only
requires the meta scope at request time; the scheduled import job later checks
`meta.<entity>.<identifier>` per item.

## Operation overview

`<resource>` ∈ {`tenant`, `statistic`, `entityright`, `processingstate`, `pickvalue`, `page`,
`export`, `documentation`, `job`, `lookup`, `entity`}:

| # | Method | Path | Parameters | Request body | Success response | Right candidate |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | GET | `/meta/<resource>/all` | `identifier` (query, required) | – | `200`, array of `<resource>` schema | `<resource>.view` |
| 2 | GET | `/meta/<resource>/query` | `identifier` (query, required) + free query params | – | `200`, array of `<resource>` schema | `<resource>.view` |
| 3 | GET | `/meta/<resource>/new` | `identifier` (query, required) | – | `200`, `<resource>` schema | `<resource>.add` |
| 4 | GET | `/meta/<resource>/byid` | `identifier`, `id` (both query, required; `id` is uuid) | – | `200`, `<resource>` schema; `404` if not found | `<resource>.view` |
| 5 | POST | `/meta/<resource>/save` | `identifier` (query, required) | `<resource>` JSON | `200` empty | `<resource>.edit` or `<resource>.<identifier>` |
| 6 | POST | `/meta/<resource>/savebatch` | `identifier` (query, required) | array of `<resource>` JSON | `200` empty | `<resource>.edit` or `<resource>.<identifier>` |
| 7 | DELETE | `/meta/<resource>/remove/{id}` | `id` (path, uuid) | – | `200` empty; `400` if remove precondition failed | `<resource>.delete` |
| 8 | POST | `/meta/<resource>/import` | `identifier` (query, required) | `multipart/form-data`, field `files` (file array) | `201` empty; schedules a job | meta scope only (job checks `<resource>.<identifier>`) |
| 9 | GET | `/meta/<resource>/export` | `identifier` (query, required) + query params | – | `200`, raw export body | `<resource>.<identifier>` |
| 10 | POST | `/meta/<resource>/exporturl` | `identifier` (query, required) | `application/x-www-form-urlencoded` fields | `200`, `ExportUrlResult` | `<resource>.<identifier>` |
| 11 | GET | `/meta/<resource>/download/{tenantId}/{id}` | `tenantId`, `id` (path, uuid) | – | `200`, file content | **anonymous** |

Rights candidates are listed as `<resource>.<right>`; the full candidate is `meta.<resource>.<right>`
because the mount sets `application="meta"` (`src/Ballware.Meta.Service/Startup.cs:428-466`).

## Query and by-id semantics

- The `identifier` selects which embedded query definition runs (see
  [concepts/metadata-model.md](../concepts/metadata-model.md));
  the conventional default is `primary`.
- Extra query parameters on `query` are passed to the query definition as parameters. A parameter
  repeated multiple times arrives merged as `|value1|value2|`
  (`TenantableEndpointHandlerFactory.cs:312-329`):

  ```text
  GET /meta/<resource>/query?identifier=primary&tag=alpha&tag=beta   →  tag = "|alpha|beta|"
  ```

- `byid` takes `identifier` **and** `id` as query parameters:
  `GET /meta/entity/byid?identifier=primary&id=<guid>` — `identifier` selects the `ByIdQuery` entry.

## Save

`save` posts a single document; `savebatch` posts an array and is checked item by item
(`TenantableEndpointHandlerFactory.cs:133-152`). Responses are empty `200` — re-read via `byid` to
get the stored document.

```bash
curl -s -X POST "https://localhost:6001/meta/entity/save?identifier=primary" \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{ "Id": "<guid>", "Application": "<application>", "Entity": "<entity>", "Meta": false, "GeneratedSchema": true }'
```

A non-`primary` identifier (e.g. `?identifier=<customSaveIdentifier>`) requires the right
`meta.<entity>.<customSaveIdentifier>` (`TenantableEndpointHandlerFactory.cs:120-124`) and runs the
matching entry of the entity's `SaveStatement` payloads.

## Remove

`remove/{id}` loads the item with the `primary` by-id query, checks `<resource>.delete`, then runs
the entity's `RemovePreliminaryCheckScript`/remove statement. If the precondition check reports
failures, the response is `400` with the joined messages as body
(`TenantableEndpointHandlerFactory.cs:154-184`).

## Export → exporturl → download

`export` returns the export payload inline; `exporturl` persists it as an `Export` record plus a
temporary file and returns an anonymous download reference valid for one day
(`TenantableEndpointHandlerFactory.cs:249-288`):

```text
GET  /meta/<resource>/export?identifier=exportjson&<query params>     raw body, media type from the export definition
POST /meta/<resource>/exporturl?identifier=exportjson                 form fields → query params → ExportUrlResult {TenantId, Id}
     form body: param1=value1&param2=value2                           Export record expires = now + 1 day (L275)
GET  /meta/<resource>/download/<tenantId>/<id>                        anonymous; file named <Query>_<timestamp><ext> (L308)
                                                                      404 when Export not found or ExpirationStamp passed (L296-299)
```

Payload/response details:

- `export` responses are declared `application/json` with a string schema in the snapshot
  (`Produces<string>()`, `TenantantableEditingEndpoint.cs:105`); at runtime the content type is the
  export definition's `MediaType` and the body is the raw export data.
- `exporturl` accepts form fields — every field becomes a query parameter of the export query
  definition (`TenantableEndpointHandlerFactory.cs:254-261`).
- `ExportUrlResult` = `{ "TenantId": "<guid>", "Id": "<guid>" }`
  (`src/Ballware.Meta.Api/Public/ExportUrlResult.cs:3-6`).

## Import

`import` uploads one or more files (form field `files`) and returns `201 Created` immediately; the
actual import runs as a Quartz job. The request itself only needs the meta scope; the job checks
`meta.<entity>.<identifier>` for each item
(`TenantableEndpointHandlerFactory.cs:188-220`,
`src/Ballware.Meta.Jobs/Internal/TenantableMetaImportJob.cs:66-70`). See
[concepts/data-integration-and-jobs.md](../concepts/data-integration-and-jobs.md) for the job chain.

```bash
curl -s -X POST "https://localhost:6001/meta/lookup/import?identifier=importjson" \
  -H "Authorization: Bearer $TOKEN" \
  -F "files=@<file>.json"
```

## Status codes

`401` (missing scope / denied right), `404` (tenant, entity metadata or item not found), `400`
(remove preconditions), `500` as `application/problem+json` — see
[concepts/tenancy-and-authorization.md](../concepts/tenancy-and-authorization.md).
