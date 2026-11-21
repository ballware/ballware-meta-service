# Tenancy and authorization

## Authentication

The service validates JWT bearer tokens against an OIDC issuer
(`src/Ballware.Meta.Service/Startup.cs:156-189`):

- `Authorization.Authority` / `Authorization.Audience` drive the JWT handler
  (`src/Ballware.Meta.Service/Configuration/AuthorizationOptions.cs:8-13`).
- `Authorization.Issuer` (optional) pins `ValidIssuer`, otherwise the authority is used
  (`Startup.cs:185-188`, `AuthorizationOptions.cs:14`).
- Inbound claim mapping is disabled (`Startup.cs:162`), so claim names appear exactly as issued.

Claims the service relies on (config keys and defaults from `AuthorizationOptions.cs:16-23`):

| Claim | Config key | Default | Used for |
| --- | --- | --- | --- |
| `tenant` | `Authorization.TenantClaim` | `tenant` | Tenant of the caller (Meta API operations) |
| `sub` | `Authorization.UserIdClaim` | `sub` | Acting user id |
| `right` | `Authorization.RightClaim` | `right` | Rights granted to the user (read by scripts/client UI) |
| `scope` | – | – | Policy selection (see below) |

## Scope → policy mapping

Two policies are built in `Startup.cs:191-204`:

- `metaApi`: required for all `/meta/...` Meta API operations; matches
  `Authorization.RequiredMetaScope` (default `metaApi`) inside the `scope` claim
  (the claim value is split on spaces, so multi-scope tokens work).
- `serviceApi`: required for all Service API operations; matches
  `Authorization.RequiredServiceScope` (default `serviceApi`).

A missing or invalid token, or a token without the required scope, yields `401 Unauthorized`.

## Tenant resolution: two styles

- **Meta API** — the tenant is derived from the `tenant` claim. Handlers bind it through
  `UserTenantId.BindAsync` (`src/Ballware.Meta.Api/Bindings/UserTenantId.cs:11-27`) or
  `IPrincipalUtils.GetUserTenandId` (`src/Ballware.Meta.Service/Configuration/AuthorizationOptions.cs:17`).
  There is no tenant path segment on Meta API routes; a caller can only act on its own tenant.
  One exception: `/meta/tenant/metadatafortenant/{tenantId}` compares the path `tenantId` against
  the caller's tenant claim and returns `403` on mismatch
  (`src/Ballware.Meta.Api/Endpoints/TenantMetaEndpoint.cs:129-147`).
- **Service API** — service-to-service callers act on a tenant given in the path
  (`{tenantId}`), because they run under their own client-credentials token
  (e.g. `src/Ballware.Meta.Api/Endpoints/EntityMetaEndpoint.cs:83-101`).

## Rights grammar

Generic editing handlers evaluate a right candidate per operation. The handler builds the candidate
as `<application>.<entity>.<operation>`; Meta API mounts use `application="meta"`,
`entity=<resource>` (`Startup.cs:426-466`), producing candidates like `meta.entity.view`:

| Operation | Right used | Source |
| --- | --- | --- |
| `all`, `query` | `view` | `ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantableEndpointHandlerFactory.cs:66,79` |
| `new` | `add` | `TenantableEndpointHandlerFactory.cs:91` |
| `byid` | `view` | `TenantableEndpointHandlerFactory.cs:110` |
| `save`, `savebatch` | `edit` — **unless** the `identifier` query parameter is present and not `primary`; then the identifier itself becomes the right | `TenantableEndpointHandlerFactory.cs:17, 122-123, 140-141` |
| `remove/{id}` | `delete` | `TenantableEndpointHandlerFactory.cs:169` |
| `export`, `exporturl` | the `identifier` itself (e.g. `exportjson`) | `TenantableEndpointHandlerFactory.cs:238, 265` |
| `import` | endpoint policy only; the scheduled job later checks `meta.<entity>.<identifier>` | `TenantableEndpointHandlerFactory.cs:188-220`, `src/Ballware.Meta.Jobs/Internal/TenantableMetaImportJob.cs:66-70` |
| `download/{tenantId}/{id}` | anonymous | `ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/TenantantableEditingEndpoint.cs:125-132` |

The `save`/`savebatch` rule lets entity metadata define dedicated save queries (for example a
`providermodel` save behind `meta.entity.providermodel` or a `state` save behind
`meta.<entity>.state`) instead of the generic `meta.<entity>.edit`.

## Script-based enforcement (the real control point)

Metadata `Rights` rows are authorization catalogs consumed by scripts and client UI — the endpoint
pipeline itself decides through two script hooks
(`ballware-shared-dotnet/src/Ballware.Shared.Api.Endpoints/EditingEndpointBuilder.cs:63-115`):

1. **Tenant level** — if tenant metadata is missing → `404`. Otherwise the tenant's
   `RightsCheckScript` is evaluated by the Jint checker with the fully built candidate right and the
   caller's claims (`ballware-shared-dotnet/src/Ballware.Shared.Authorization.Jint/Internal/JavascriptTenantRightsChecker.cs:8-25`):

   ```javascript
   // available variables inside Tenant.RightsCheckScript (synthetic pattern):
   // right    -> "meta.<entity>.<operation>"   (fully composed candidate)
   // userinfo -> claim map of the caller
   if (right.startsWith('meta.')) {
     return true;                       // allow all meta-catalog rights, for example
   }
   return userinfo.rights?.includes(right) ?? false;
   ```

2. **Entity level** — if entity metadata for the tenant is missing → `404`. Otherwise the entity's
   `CustomScripts.extendedRightsCheck` runs with `application`, `entity`, `right` (the bare
   operation or identifier), `param` (item or query parameters), `result` (tenant verdict) and
   `userinfo`, and can override the tenant verdict
   (`ballware-shared-dotnet/src/Ballware.Shared.Authorization.Jint/Internal/JavascriptEntityRightsChecker.cs:8-29`).
   Entity scripts commonly delegate to other applications' rights or build synthetic rights such as
   `<application>.<entity>.<field>.<right>`.

If the combined verdict is `false`, the handler returns `401 Unauthorized`
(`EditingEndpointBuilder.cs:111-113`) — a rights denial is not `403`.

State transitions add a third script hook: an entity's `StateAllowedScript` is evaluated with
`state`, `hasRight(...)`, `hasAnyRight(...)` against the user's `right` claims
(`JavascriptEntityRightsChecker.cs:31-46`).

## Expected status codes

| Situation | Status | Source |
| --- | --- | --- |
| Missing/invalid token or missing scope | `401` | `Startup.cs:191-204` |
| Right denied by tenant/entity scripts | `401` | `EditingEndpointBuilder.cs:111-113` |
| Tenant or entity metadata not found for the right check | `404` | `EditingEndpointBuilder.cs:67-71, 97-101` |
| Item not found (`byid`, `selectbyid`, …) | `404` | e.g. `src/Ballware.Meta.Api/Endpoints/EntityMetaEndpoint.cs:111-114` |
| Remove precondition failed (`RemovePreliminaryCheckScript`) | `400` with the messages as body | `TenantableEndpointHandlerFactory.cs:177-179` |
| Caller tenant ≠ path tenant on `/meta/tenant/metadatafortenant/{tenantId}` | `403` | `TenantMetaEndpoint.cs:141-143` |
| Unhandled exception | `500` `application/problem+json` | `Startup.cs:394-419` |
