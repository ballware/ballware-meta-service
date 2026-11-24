# Plan: `docs/` folder for ballware-meta-service (API + payloads)

## Goal

Add a self-contained `docs/` folder (English, tool-agnostic Markdown) that explains the service's
functionality with a strong focus on **API calls and data payloads**, based on the OpenAPI
specifications, so that `ballware-docs` can later ingest per-service documentation directly from the
service repos (same pattern as `ballware-angular/docs/metadata/`). `README.md` keeps the setup role and
gains a short pointer to `docs/`.

## Verified context (evidence, do not re-derive)

- Two OpenAPI documents exist at runtime: `meta` (150 paths, 25 component schemas) and `service`
  (17 paths, 24 schemas), OpenAPI `3.0.4`, titles "ballware Meta API" / "ballware Service API"
  (`src/Ballware.Meta.Service/Startup.cs:339-383`, UI wiring `Startup.cs:480-497`).
- Nothing is checked in: `git ls-files` shows no `swagger.json`; `src/Ballware.Meta.Service.Client/`
  contains only `nswag.json` + csproj (NSwag generates `MetaServiceClient.cs` from `swagger.json` in CI).
- CI already generates the spec **offline, without a database**
  (`.github/workflows/publish-packages.yml:29-33, 50-62`):
  `ASPNETCORE_ENVIRONMENT=Swagger` + `dotnet tool run swagger tofile --output
  ../Ballware.Meta.Service.Client/swagger.json ./bin/Release/net10.0/Ballware.Meta.Service.dll service`,
  tool pinned to `Swashbuckle.AspNetCore.Cli 10.1.3`. `appsettings.Swagger.json` supplies fake
  Authority/clients; `InitializationWorker` (EF migration at startup,
  `src/Ballware.Meta.Data.Ef.Postgres/Internal/InitializationWorker.cs:24-28`) only runs when the host
  actually starts, so `swagger tofile` needs no Postgres/Keycloak. CI exports only the `service`
  document; the `meta` document must additionally be exported for the docs.
- Snapshot freshness check passed: the `ballware-docs` snapshot
  (`~/Documents/GitHub/ballware-docs/openapi/meta/{meta,service}.json`) exposes exactly the 38
  properties of `src/Ballware.Meta.Data/Public/EntityMetadata.cs` → DTO-derived tables and spec
  components agree with current `main`.
- Generic CRUD contract: 11 operations per resource
  (`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:22-132`),
  handler semantics in `TenantableEndpointHandlerFactory.cs` (default identifier `primary` L17,
  rights `view/add/edit/delete` L19-22, `identifier != "primary"` turns the identifier itself into the
  required right L122-123/L140-141, multi-value query params encoded as `|a|b|` L224-235, import =
  multipart upload + Quartz job trigger L186-207, `exporturl` form-encoded → `ExportUrlResult` +
  anonymous `download/{tenantId}/{id}` with 1-day expiry L209-236, 238-256).
- Meta resources are mounted with `application="meta"`, `entity=<resource>`
  (`Startup.cs:426-466`) → rights strings `meta.<resource>.<right>`.
- Tenant resolution: meta API reads the `tenant` claim
  (`src/Ballware.Meta.Service/Configuration/AuthorizationOptions.cs:17`, `src/Ballware.Meta.Api/Bindings/UserTenantId.cs`);
  service API takes explicit `{tenantId}` path segments. Scope policies `metaApi`/`serviceApi`
  (`Startup.cs:191-204`). JSON is PascalCase (`Startup.cs:52-60, 221-226`); 500 →
  `application/problem+json` (`Startup.cs:394-419`).
- Layout/lookup/script payloads are **JSON strings embedded in the envelope**
  (`src/Ballware.Meta.Data/Public/EntityMetadata.cs:172-185`); child collections `States`, `Rights`,
  `Pickvalues` are real arrays (L187-189).
- Schema provisioning side effects on save/remove:
  `src/Ballware.Meta.Service/Extensions/GenericSchemaEntityRepositoryHook.cs:20-43`
  (identifiers `primary|importjson|providermodel` + `GeneratedSchema` + `ProviderModelDefinition` →
  `TenantCreateOrUpdateEntitySchemaForTenant`; remove → `TenantDropEntitySchemaForTenant`), plus
  `GenericSchemaTenantRepositoryHook`.
- Service API payload shapes: `JobCreatePayload {Identifier, Options, Scheduler}`,
  `JobUpdatePayload {Id, Result, State}`, `ExportCreatePayload {Application, Entity, ExpirationStamp,
  MediaType, Query}` (from the service snapshot).

## Additional research inputs (added by user)

- The previously inaccessible export was copied to
  `/Users/frank/Documents/GitHub/ballware-fwpro/meta/metadata-entities.json`. It is readable now and is a
  JSON array of 20 `EntityMetadata` objects for built-in applications (`ai`, `identity`, `meta`). Use it
  as production-derived research input only.
- `/Users/frank/Documents/GitHub/ballware-fwpro` contains split production tenant metadata files for one
  tenant, for example `tenant/ffw_pr_oldendorf.json`, `entity/unit.json`, `entity/location.json`,
  `lookup/unitLookup.json`, `statistic/unitPersonAvailability.json`, `page/myPage.json`, and
  `documentation/my.json`. Use these for pattern discovery and realism only.
- Observed production metadata conventions to abstract for docs:
  - Many document exports are JSON arrays wrapping a single object.
  - `EntityMetadata` payload strings such as `ListQuery`, `ByIdQuery`, `NewQuery`, `SaveStatement`,
    `Lookups`, `GridLayout`, `EditLayout`, `CustomScripts`, `CustomFunctions`, `Templates`, and
    `ProviderModelDefinition` are embedded JSON/JSON5-style strings, not direct OpenAPI objects.
  - Query identifiers commonly include `primary` plus semantic identifiers such as `name`, `report`,
    `exportjson`, `importjson`, `state`, `context`, `entity`, or tenant-specific variants.
  - Custom function ids can create rights names such as `<application>.<entity>.<functionId>`; for
    example production entity right rows use application-prefixed right identifiers.
  - `KeyColumn` can be null or an explicit user key column; `Meta`, `GeneratedSchema`, and `NoIdentity`
    control persistence behavior.
  - Embedded arrays may contain `__KEY__` entries. Treat `__KEY__` as a storage/export/metadata-editor
    artifact unless the generated OpenAPI snapshot or DTO contract explicitly confirms it.
  - Built-in metadata examples commonly use `Meta:true`, `GeneratedSchema:false`, null `KeyColumn`, and
    central metadata persistence; tenant business examples in `ballware-fwpro` commonly use `Meta:false`
    and `GeneratedSchema:true` with provider-model custom columns. State lifecycle examples show
    `StateColumn`, `States`, successor state ids, and rights such as `<application>.<entity>.visiblestate.<state>`.
  - Metadata entities are not identical to Meta API resources. The current generated Meta OpenAPI exposes
    only `documentation`, `entity`, `entityright`, `export`, `job`, `lookup`, `page`, `pickvalue`,
    `processingstate`, `statistic`, and `tenant` under `/meta/...`. Metadata exports may contain other
    entities (`document`, `mlmodel`, `notification`, `subscription`, `ai`, `identity`); do not invent
    `/meta/<entity>` endpoints unless the OpenAPI snapshot contains them.
- Rights enforcement nuance: generic endpoint handlers construct the right candidate as
  `<application>.<entity>.<operation>`; meta API mounts use `application="meta"`, `entity=<resource>`,
  producing candidates like `meta.entity.view`. Tenant and entity `RightsCheckScript`s decide whether
  that candidate is allowed; metadata `Rights` rows are application authorization catalogs used by
  scripts/client UI, not automatic meta-API policy enforcement.

## Decisions (agreed with user)

1. Commit OpenAPI snapshots into `docs/openapi/` **and** provide a fetch script; no CI drift job.
2. Structure: `docs/README.md` + `docs/concepts/` + `docs/api/` + `docs/openapi/` (angular-docs style).
3. Language: English.
4. MCP endpoint `/meta/mcp`: short overview only (not in OpenAPI).
5. Tool-agnostic plain Markdown: relative links, no MDX/JSX, no Docusaurus admonitions or sidebars,
   no `sidebar.ts`; ASCII diagrams (as in `ballware-angular/docs`); no frontmatter needed for import.
6. Snapshot bootstrap: generate locally with the CI mechanism (no DB needed), not copied from
   `ballware-docs`.
7. Production metadata exports (`metadata-entities.json` and `ballware-fwpro`) are research inputs for
   realistic payload patterns and best-practice conventions. They must not become public documentation
   content: no copied tenant names, GUIDs, emails, schemas, SQL snippets, captions, production rights
   strings, server config, subscription/notification examples, or direct citations of those files.

## Target tree

```
docs/
  README.md                     index + contents table + scope note
  concepts/
    architecture.md
    tenancy-and-authorization.md
    metadata-model.md
    data-integration-and-jobs.md
    mcp-endpoint.md
  api/
    README.md                   API surface + conventions
    generic-crud.md             11 generic operations per resource
    meta-api-resources.md       resource-specific operations (meta doc)
    service-api.md              17 service operations
    schemas.md                  field tables for 25 meta + 24 service schemas
    examples.md                 runnable curl + payload sequences
  openapi/
    README.md                   how the snapshot is produced/refreshed
    fetch-spec.sh               swagger-to-file driver
    meta.json                   generated snapshot (expect 150 paths)
    service.json                generated snapshot (expect 17 paths)
    manifest.json               provenance: tool version, tag, fetchedAt, pathCount
```

## Tasks (ordered)

0. **Research pass on production metadata examples** before writing docs:
   - Inspect `/Users/frank/Documents/GitHub/ballware-fwpro/meta/metadata-entities.json` for built-in
     metadata patterns: array envelope, embedded JSON strings, custom function ids, import/export rights,
     state rights, provider-model shapes, and the distinction between metadata entities and Meta API paths.
   - Inspect representative `ballware-fwpro` paths: `entity/unit.json`, `entity/location.json`,
     `tenant/ffw_pr_oldendorf.json`, `lookup/unitLookup.json`, `statistic/unitPersonAvailability.json`,
     `page/myPage.json`, `documentation/my.json`.
   - Extract only reusable conventions: envelope arrays, query identifier families, embedded JSON-string
     fields, custom-function/import-export rights, lookup/statistic/page/documentation payload shapes,
     state/picklist/statistics usage patterns.
   - Convert findings into synthetic examples with placeholders (`<application>`, `<entity>`, `<guid>`,
     `<token>`, `<tenantId>`) and do not mention the production tenant or file contents in public docs.
1. **Snapshot generator script** `docs/openapi/fetch-spec.sh`
   - `dotnet build src/Ballware.Meta.Service -c Release`
   - ensure tool availability, mirroring CI without polluting the repo:
     `dotnet tool install Swashbuckle.AspNetCore.Cli --version 10.1.3 --global` guarded by a
     `command -v swagger` check (alternative documented: `dotnet new tool-manifest` +
     `dotnet tool update`, which is what CI does).
   - run twice with `ASPNETCORE_ENVIRONMENT=Swagger`, `meta` and `service`:
     `swagger tofile --output docs/openapi/<doc>.json src/Ballware.Meta.Service/bin/Release/net10.0/Ballware.Meta.Service.dll <doc>`
   - validate: file parses as JSON, `openapi` starts with `3.`, `paths` non-empty; fail otherwise.
   - write `manifest.json`: `source: "Swashbuckle.AspNetCore.Cli swagger tofile"`, tool version,
     `environment: Swagger`, `serviceVersion: $(git describe --tags)` (GitVersion tags like `v1.4.0`),
     `fetchedAt`, and per-doc `{openapi, title, pathCount, schemaCount}`.
   - Script must be runnable from the repo root and idempotent.
2. **Generate the two snapshots** by executing the script; commit `meta.json` (150 paths),
   `service.json` (17 paths), `manifest.json`.
3. **`docs/README.md`**: purpose + scope note ("backend view; the frontend consumption contract lives in
   `ballware-angular/docs/metadata`"), contents table, quick facts (ports 5001 HTTP / 6001 HTTPS dev,
   5001 in compose; documents `meta`/`service`; Swagger UI `/swagger`), link to `README.md` for setup,
   link to `docs/openapi/README.md` for regeneration, request-flow ASCII diagram
   (client → JWT → policy → handler → rights check → repository → EF/tenant schema → Storage/Schema client).
4. **`docs/concepts/architecture.md`**: project layout (from `README.md`), request pipeline with
   `Startup.cs` line citations, mounted-route table (resource → paths → tag → rights prefix from
   `Startup.cs:426-466`), caching (`Storage.EnableCaching`, `Cache.*`, `Ballware.Meta.Data.Caching`),
   Quartz/hosted worker, error contract (`ProblemDetails`), CORS, `Mcp` section.
5. **`docs/concepts/tenancy-and-authorization.md`**: OIDC/JWT validation, scope→policy mapping, claim
    names (`tenant`, `sub`, `right`) and their config keys, meta-API claim-derived tenant vs
    service-API explicit `{tenantId}`, rights grammar `meta.<resource>.<view|add|edit|delete|<functionId>>`
    with the `identifier`→right rule, Jint extended check via entity `CustomScripts.extendedRightsCheck`,
    and expected status codes (401 missing scope/token, 404 not found, 400 failed remove preconditions).
    Explain script-based enforcement as the real control point: tenant `RightsCheckScript` receives the
    computed right candidate plus `userinfo` claims; entity scripts can delegate to other application
    rights or build synthetic rights such as `<prefix>.<entity>.<field>.<right>`. Use synthetic script
    patterns only; do not copy production `origin`, `ai`, tenant scripts, or tenant-specific right names.
6. **`docs/concepts/metadata-model.md`**: document types and their role: `EntityMetadata` (38-field
   table with Type/Required/Description, citing `src/Ballware.Meta.Data/Public/EntityMetadata.cs`),
   `Tenant`, `Page`, `Lookup`, `Pickvalue`, `ProcessingState`, `Statistic`, `Documentation`, `Export`,
   `Job`, `EntityRight` (sources `src/Ballware.Meta.Data/Public/*.cs`); embedded-JSON-string convention
    with one escaped + one unescaped example; child collections `States`/`Rights`/`Pickvalues`; flags
    `Meta`, `GeneratedSchema`, `NoIdentity`, `KeyColumn` (user-defined unique key, issue #60). Add a
    short "observed production conventions" subsection using anonymized patterns only: multiple query
    identifiers behind embedded arrays, semantic query ids for export/import/name/report/context flows,
    custom-function ids that map to rights names, and `KeyColumn` null vs explicit user key column. Do
    not copy production identifiers or content from `metadata-entities.json` / `ballware-fwpro`.
7. **`docs/concepts/data-integration-and-jobs.md`**: tenant-scoped persistence (`ListQuery`,
   `ByIdQuery`, `NewQuery`, `ScalarValueQuery`, `SaveStatement`, `RemoveStatement`, `Indices` as the
   query definitions executed per tenant), schema provisioning via the repository hooks (citation
   `GenericSchemaEntityRepositoryHook.cs:20-43`), Storage client for temporary/export files
   (`Startup.cs:251-261`), client-credentials token flow for Storage/Schema (`Startup.cs:229-249`),
   import job (`JobKey.Create("import", entity)`, `JobDataMap` keys `tenantId/userId/identifier/claims/file/jobId`),
   `JobStates` lifecycle and the Service API job endpoints used by other services.
8. **`docs/concepts/mcp-endpoint.md`** (short): `/meta/mcp` (`Startup.cs:474-478`), `Mcp` options
   (`RequiredMcpScope` defaults to the meta scope, `ResourceUri`), registered tool families
   (`Startup.cs:324-333`: tenant, page, entity, lookup, pickvalue, statistic, processingstate),
   explicit note that MCP is **not** part of the OpenAPI documents.
9. **`docs/api/README.md`**: two documents and group/tag naming (`WithGroupName`/`WithTags`,
   `CustomSchemaIds` strips the `Dto` suffix, `Startup.cs:343-365`), operation-id convention
   (`<Prefix><Name>`, e.g. `EntitySelectListRights`), PascalCase bodies, `identifier` query-parameter
   semantics table (default `primary`; absent on `remove/{id}` and `download`), anonymous download,
   `Accept`/`Content-Type`, token acquisition sketch (client credentials for service callers,
   authorization code + PKCE for Swagger UI).
10. **`docs/api/generic-crud.md`**: one canonical section for all 11 operations with method, path,
     query/body, response schema, rights and example payloads. State explicitly: endpoint policy is
     `metaApi` for meta resources; most handlers then evaluate a right candidate through tenant/entity
     scripts (`all`/`query`/`byid` use view, `new` uses add, `remove/{id}` uses delete, `save`/`savebatch`
     use edit unless `identifier` is non-primary, `export`/`exporturl` use the identifier as the right).
     `import` itself only requires the meta scope and schedules a job; `TenantableMetaImportJob` later
     checks `meta.<entity>.<identifier>`. Explicit sub-sections for the
    `export` (raw body, media type from `Export.MediaType`) → `exporturl`
    (`application/x-www-form-urlencoded` form fields become query params → `ExportUrlResult
    {TenantId, Id}`) → `download/{tenantId}/{id}` (anonymous, filename `<Query>_<timestamp><ext>`,
    404 when `ExpirationStamp` passed) flow; `import` multipart (multipart field = file, response
    `201 Created`, side effect job); `savebatch` array body; multi-value param encoding `|a|b|`.
11. **`docs/api/meta-api-resources.md`**: cover only OpenAPI-backed meta resources (`documentation`,
     `entity`, `entityright`, `export`, `job`, `lookup`, `page`, `pickvalue`, `processingstate`,
     `statistic`, `tenant`). `entityright` and `export` are generic-CRUD-only; note explicitly that
     metadata exports may contain entities such as `document`, `mlmodel`, `notification`, `subscription`,
     `ai`, or `identity`, but those are out of this API surface unless present in the OpenAPI snapshot.
     Then list, per resource, the operations beyond generic CRUD with payload
    tables for the select-list DTOs (`src/Ballware.Meta.Data/SelectLists/*.cs`,
    `src/Ballware.Meta.Api/Public/*.cs`): entity (`metadataforentity/{identifier}`, `selectlist`,
    `selectbyid`, `selectbyidentifier`, `selectlistrights`), page (`pagedataforidentifier`),
    statistic (`metadataforidentifier`), tenant (`metadatafortenant/{tenantId}`, `allowed`),
    documentation (`documentationforentity`, `documentationforentityandfield`, `selectlist`,
    `selectbyid`), lookup (`selectlist`, `selectbyid`), pickvalue
    (`selectlistforentityandfield`, `selectbyvalueforentityandfield`), processingstate (`selectlist`,
    `selectbyid`, `selectlistforentity`, `selectbystateforentity`,
    `selectlistallsuccessorsforentityandstate`), job (`pendingjobsforuser`).
12. **`docs/api/service-api.md`**: table of all 17 operations (method, path, params, response schema,
    purpose) plus payload detail for `ExportCreatePayload`, `JobCreatePayload`, `JobUpdatePayload`,
    `ServiceEntity` (+ `ServiceEntityCustomFunction/Options/Scripts`,
    `ServiceEntityQueryEntry/Parameter/QueryResultColumn`, `EntityCustomFunctionTypes`, `JobStates`) and
    `ServiceTenant` + `ServiceTenantReportDatasource{Definition,Table,Relation}`,
    `Lookup`, `Statistic`, `EntitySelectListEntry`, `ProcessingStateSelectListEntry`,
    `PickvalueSelectEntry`, `TenantSelectListEntry` (sources `src/Ballware.Meta.Api/Public/*.cs` and
    the `service.json` components).
13. **`docs/api/schemas.md`**: one field table per component schema of both snapshots (25 meta +
    24 service), columns Field/Type/Nullable/Notes, each table headed with its source
    (`docs/openapi/meta.json#/components/schemas/<Name>` + DTO file:line). Enum-valued fields
    (`JobStates`, `EntityCustomFunctionTypes`) documented with their serialized values
    (`add|edit|default_add|default_view|default_edit|external|export|import`).
14. **`docs/api/examples.md`**: copy-pasteable curl blocks (placeholder `$TOKEN`, `https://localhost:6001`)
    for: tenant metadata, entity envelope read, entity save with embedded JSON-string layouts,
    `query?identifier=...&param=...`, `byid`/`new`, `savebatch`, `export` → `exporturl` → `download`,
    `import` multipart, Service API export create/fetch, job create/update, lookup metadata for tenant,
     pickvalue select-by-value, processingstate successors, tenant report datasources.
     Every request must be validated against the snapshot before it is written. Use production metadata
     examples only as realism checks; all examples must be synthetic and sanitized, with placeholder
     tenants/GUIDs/entities/queries and no copied fwpro values.
15. **`docs/openapi/README.md`**: how the snapshot is produced (exact commands), why
    `ASPNETCORE_ENVIRONMENT=Swagger` is required, that `appsettings.Swagger.json` uses fake
    Authority/clients so the `oidc` security definition in the snapshot is not a real endpoint, and that
    a developer `appsettings.local.json` would leak into the snapshot's OIDC URL → regenerate from a
    clean checkout. Path/schema counts, regeneration cadence ("regenerate after endpoint changes"),
    and the mapping snapshot document → docs pages.
16. **`README.md`**: insert a short `## Documentation` section after "What the Meta Service provides"
    pointing to `docs/README.md`, `docs/api/README.md` and `docs/openapi/` (committed OpenAPI
    snapshots), and reword the existing "Use Swagger UI …" line to mention that the generated
    documents are also available at `/swagger/meta/swagger.json` and `/swagger/service/swagger.json`.
    No other README changes.

## Content rules

- English; plain `.md`; relative links only; ASCII diagrams; tables + JSON code fences; source
  citations as `path:line` (angular-docs style). No Docusaurus directives, no frontmatter, no `.mdx`.
- Never invent endpoints/fields: every documented path must exist in
  `docs/openapi/{meta,service}.json`; meta has no top-level `count` or `newquery` route segment, but
  generic `query` may accept metadata query identifiers such as `count` when those identifiers are defined
  in the entity metadata. Do not copy endpoint lists blindly from `ballware-angular` docs.
- Keep secrets and production data out: no real client secrets, no real IdP URLs, no tenant names, GUIDs,
  emails, schemas, SQL snippets, captions, rights strings, server config, or file paths from production
  exports in public docs. Research files may inform synthetic examples but are not to be quoted or cited
  publicly.
- Payload emphasis: request/response bodies and identifier semantics over prose.

## Out of scope

- CI changes (no job that regenerates/verifies the snapshot; extending `publish-packages.yml` to also
  export the `meta` document is a documented optional follow-up, not part of this plan).
- Any change to `src/Ballware.Meta.Service.Client/nswag.json` or client generation.
- Full MCP tool schemas, Docusaurus sidebars/frontmatter, PDF/HTML publishing, translation.

## Risks / gotchas

- `swagger tofile` must resolve the host without starting it; CI proves this for `service`. Exporting
  `meta` is new for this repo — verify `paths` count 150 and that both documents are produced; if the
  tool cannot resolve the entry point, fall back to `dotnet run` with
  `ASPNETCORE_ENVIRONMENT=Swagger` plus a reachable Postgres and fetch
  `http://localhost:5001/swagger/<doc>/swagger.json` (document this fallback in
  `docs/openapi/README.md`).
- Tool version must stay `10.1.3` to match CI/Swashbuckle 10.1.3; otherwise snapshot formatting drifts.
- Production research files are sensitive: `ballware-fwpro` includes tenant name, emails, GUIDs, schemas,
  provider SQL, navigation, subscriptions, and tenant scripts. The docs must not leak any of that.
- Do not treat every metadata export entity as a Meta API resource. The OpenAPI snapshot is the source of
  truth for `/meta/...` paths; exports may include central/admin metadata entities that are not exposed by
  the current Meta API.
- Local `appsettings.local.json` influences the snapshot's `openIdConnectUrl` (documented in
  `docs/openapi/README.md`).
- Docs/spec drift: the snapshot is a manual artifact; regeneration is a documented human step.

## Validation

1. `jq -r '.paths|keys|length' docs/openapi/meta.json` → 150; `…/service.json` → 17;
   `jq -r '.openapi' …` → `3.*`.
2. Documented-path audit: extract every `/meta/...` path literal from `docs/api/*.md` and confirm each
   exists in the snapshots (`jq -r '.paths|keys[]'`), no extras.
3. Schema audit: every `#/components/schemas/<Name>` referenced in `docs/api/schemas.md` exists in the
   corresponding snapshot; counts 25 (meta) and 24 (service).
4. Link audit: `test -f` each relative link target in `docs/**` and in the new `README.md` section.
5. Idempotency: re-run `docs/openapi/fetch-spec.sh` → `git diff` shows only `manifest.json` `fetchedAt`
   (no spec churn).
6. Privacy/sanitization audit: search docs and README additions for obvious production leakage such as
   tenant names (`Pr. Oldendorf`, `FWW`), email domains (`@gmx`, `@web`, `@gmail`, `@outlook`),
   `ballware-fwpro`, `metadata-entities.json`, fwpro GUIDs, subscription file names, and fwpro schema/table
   names. Any match must be replaced with synthetic placeholders or removed.
7. No code touched: `dotnet build ballware-meta-service.sln` and `dotnet test` must stay green
   (docs only).

## Open questions

- Should CI additionally export the `meta` document (one extra `swagger tofile` step) so the committed
  snapshot can be refreshed automatically later? Deferred, out of scope here.
- Should `ballware-docs` read `docs/**` + `docs/openapi/*.json` from this repo (static import) or keep
  fetching live specs? Import mechanics live in `ballware-docs`, not in this plan.
- No current blocker. The production metadata export is now available at
  `/Users/frank/Documents/GitHub/ballware-fwpro/meta/metadata-entities.json` and can be used for pattern
  extraction only.
