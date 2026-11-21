# MCP endpoint (overview)

The service also exposes a Model Context Protocol endpoint so MCP clients can discover tenant
context, entities, pages, lookups, pickvalues, statistics and processing states as tools.

- Mounted at `/meta/mcp` when MCP options are configured
  (`src/Ballware.Meta.Service/Startup.cs:474-478`); the route is only registered when
  `Mcp.Enabled` is true (`ballware-shared-dotnet/src/Ballware.Shared.Mcp.Endpoints/McpEndpoint.cs:13`).
- Options (`Mcp` section): `Enabled` (default `false`), `RequiredMcpScope`, `ResourceUri`
  (used for OAuth protected-resource metadata). Without a `Mcp` section, `RequiredMcpScope` defaults
  to the Meta API scope and `ResourceUri` stays empty (`Startup.cs:112-119`).
- Registered tool families (`Startup.cs:324-333`): tenant, page, entity, lookup, pickvalue,
  statistic, processingstate.
- MCP authentication uses its own scheme with the configured scope
  (`Startup.cs:159, 476`).

**MCP is not part of the OpenAPI documents** `meta` and `service` — it does not appear in
[docs/openapi/meta.json](../openapi/meta.json). Detailed MCP tool schemas are out of scope here.
