# OpenAPI snapshots

The committed files in this folder are the source of truth the API pages were written against:

| File | Content |
| --- | --- |
| [meta.json](meta.json) | `ballware Meta API` — 150 paths, 25 component schemas, OpenAPI `3.0.4` |
| [service.json](service.json) | `ballware Service API` — 17 paths, 24 component schemas, OpenAPI `3.0.4` |
| [manifest.json](manifest.json) | Provenance: generator tool + version, environment, git describe reference, `fetchedAt`, per-document openapi/title/pathCount/schemaCount |
| [fetch-spec.sh](fetch-spec.sh) | Generator script |

## How the snapshot is produced

Generation runs **offline from the built assembly** — no database, no IdP. The CI pipeline uses the
same mechanism for the client build
(`.github/workflows/publish-packages.yml:29-33, 50-62`); this folder extends it to also export the
`meta` document.

Run from the repository root (idempotent — it rebuilds and rewrites the three files):

```bash
docs/openapi/fetch-spec.sh
```

What it does:

1. `dotnet build src/Ballware.Meta.Service --configuration Release`
2. Ensures the tool is available — global install guarded by a PATH check:
   `dotnet tool install Swashbuckle.AspNetCore.Cli --version 10.1.3 --global`
   (the CI alternative: `dotnet new tool-manifest && dotnet tool update Swashbuckle.AspNetCore.Cli --version 10.1.3`,
   invoked via `dotnet tool run swagger …`)
3. Exports both documents with the Swagger environment:

   ```bash
   export ASPNETCORE_ENVIRONMENT=Swagger
   export Authorization__Authority=https://localhost
   cd src/Ballware.Meta.Service
   swagger tofile --output ../../docs/openapi/meta.json bin/Release/net10.0/Ballware.Meta.Service.dll meta
   swagger tofile --output ../../docs/openapi/service.json bin/Release/net10.0/Ballware.Meta.Service.dll service
   ```

4. Validates each file (parses as JSON, `openapi` starts with `3.`, `paths` non-empty) and writes
   `manifest.json`.

The `swagger tofile` tool resolves the host from the assembly **without starting it**, so the EF
initialization worker never runs — no Postgres, no Keycloak needed.

## Why `ASPNETCORE_ENVIRONMENT=Swagger`

`appsettings.Swagger.json` (`src/Ballware.Meta.Service/appsettings.Swagger.json`) supplies **fake**
Authority/clients (`https://localhost`, `fake*` clients/secrets). Consequently the `oidc` security
definition in the snapshot points at a non-existent endpoint:

```json
"oidc": { "type": "openIdConnect", "openIdConnectUrl": "https://localhost/.well-known/openid-configuration" }
```

That URL is a placeholder, **not** a real IdP discovery endpoint. Do not wire clients to it.

> **Leak warning:** configuration order is
> `appsettings.json` → `appsettings.{Environment}.json` → `appsettings.local.json` → environment
> variables (`src/Ballware.Meta.Service/Program.cs:11-15`). A developer
> `appsettings.local.json` overrides the Swagger values — including the Authority — and its real IdP
> URL would be baked into the committed snapshot. The script therefore pins
> `Authorization__Authority=https://localhost` as an environment override. When regenerating by hand
> (or in CI), generate from a clean checkout without `appsettings.local.json`.

## Tool version pin

Keep `Swashbuckle.AspNetCore.Cli` at **10.1.3** — it matches the service's Swashbuckle packages
(`src/Ballware.Meta.Service/Ballware.Meta.Service.csproj:45-46`) and the CI tool pin
(`publish-packages.yml:32`). Other versions may reformat the documents and create spurious diffs.

## Refresh cadence and drift

The snapshot is a manual artifact: regenerate after endpoint changes (new/renamed routes, DTO
changes, Swagger config). A re-run should only change `manifest.json`'s `fetchedAt`; if
`meta.json`/`service.json` also change, review the diff — it is an API surface change and the API
pages here should be reviewed against it. Regenerating does not require touching any source code.

Optional follow-up (not part of this setup): extend CI to export `meta.json` next to `service.json`
so refreshes can be automated.

## Snapshot ↔ docs mapping

| Snapshot | Documented by |
| --- | --- |
| `meta.json` paths + schemas | [../api/generic-crud.md](../api/generic-crud.md), [../api/meta-api-resources.md](../api/meta-api-resources.md), meta tables in [../api/schemas.md](../api/schemas.md) |
| `service.json` paths + schemas | [../api/service-api.md](../api/service-api.md), service tables in [../api/schemas.md](../api/schemas.md) |
| both | [../api/examples.md](../api/examples.md) (every example path exists in the snapshots) |

## Live-server fallback

If `swagger tofile` ever fails to resolve the entry point, generate from a running instance instead
(needs a reachable database per `Storage.AutoMigrations`):

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/Ballware.Meta.Service
curl -s http://localhost:5001/swagger/meta/swagger.json -o docs/openapi/meta.json
curl -s http://localhost:5001/swagger/service/swagger.json -o docs/openapi/service.json
```

Prefer the offline path; a live-server export inherits the developer's local configuration.
