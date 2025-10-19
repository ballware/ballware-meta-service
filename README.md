# Ballware Meta Service

[![Build & Publish (main)](https://github.com/ballware/ballware-meta-service/actions/workflows/publish-packages.yml/badge.svg?branch=main)](https://github.com/ballware/ballware-meta-service/actions/workflows/publish-packages.yml)
[![CI Tests (main)](https://github.com/ballware/ballware-meta-service/actions/workflows/sonarqube-latest.yml/badge.svg?branch=main)](https://github.com/ballware/ballware-meta-service/actions/workflows/sonarqube-latest.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=coverage)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=bugs)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=vulnerabilities)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=security_rating)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ballware_ballware-meta-service&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=ballware_ballware-meta-service)

This repository contains the Ballware Meta Service – the component that stores and serves metadata used by the Ballware platform. It exposes a set of HTTP APIs to manage tenant-aware metadata such as entities, pages, lookups, pick values, exports, documentation, jobs, and statistics.

For a complete, multi-service installation (Keycloak, Storage, Schema, etc.), please use the umbrella repository: https://github.com/ballware/ballware-docker-compose. This README focuses solely on the specifics of the Meta Service.


- Tech stack: .NET 8, ASP.NET Core, EF Core (PostgreSQL or SQL Server), Serilog, Quartz, AutoMapper
- AuthN/Z: OAuth2/OIDC with JWT Bearer, scope-based authorization (metaApi, serviceApi)
- Caching: In-memory or Redis
- Clients: Calls the Storage Service and the Schema Service via OAuth2 client credentials
- API docs: Swagger/OpenAPI


## Repository structure

- `src/Ballware.Meta.Service/` – Web service host (Startup, Program, appsettings)
- `src/Ballware.Meta.Api/*` – API endpoints and mappings
- `src/Ballware.Meta.Data.*/*` – Data access implementations (EF Core, repositories, migrations)
- `src/Ballware.Meta.Jobs/*` – Background jobs (Quartz)
- `src/Ballware.Meta.Caching/*` – Caching abstractions and implementations
- `test/*` – Unit/integration tests


## What the Meta Service provides

The service hosts two logical API surfaces per resource:

- Meta API – CRUD-like endpoints for metadata management
- Service API – read-oriented endpoints used by other services/clients

Mounted routes (prefix → resource examples):

- `/meta/tenant` – tenants
- `/meta/statistic` – statistics
- `/meta/entityright` – entity rights
- `/meta/processingstate` – processing states
- `/meta/pickvalue` – pick values
- `/meta/page` – pages
- `/meta/export` – export definitions
- `/meta/documentation` – documentation
- `/meta/job` – job definitions
- `/meta/lookup` – lookups
- `/meta/entity` – entity metadata

Use Swagger UI to explore the concrete operations and models.


## Quick start (local development)

Prerequisites:

- .NET SDK 8.0+
- PostgreSQL or SQL Server for metadata storage
- (Optional) Redis for distributed caching
- An OIDC provider (e.g., Keycloak) reachable by the service

Steps:

1) Trust the local ASP.NET Core development certificate

- macOS/Linux/Windows: run
  - `dotnet dev-certs https --trust`

2) Create `src/Ballware.Meta.Service/appsettings.local.json`

- A working sample is present in the repository; adjust Authority, connection strings, and client credentials to your setup (see Configuration section below). No PFX export is required; Kestrel will use the system dev certificate automatically in Development.

3) Start the service

- From the repo root:
  - `dotnet restore`
  - `dotnet build`
  - `dotnet run --project src/Ballware.Meta.Service`

By default in Development the service listens on:

- HTTP: `http://localhost:5001`
- HTTPS: `https://localhost:6001`

4) Open Swagger UI

- Navigate to `https://localhost:6001/swagger` (see Swagger section for usage with OIDC).


## Configuration

Configuration is loaded in this order: `appsettings.json` → `appsettings.{Environment}.json` → `appsettings.local.json` → environment variables.

Key sections used by the service:

- Authorization
- Storage (EF Core)
- Cache
- Swagger
- Cors
- StorageClient (OAuth2 client credentials for the Storage Service)
- SchemaClient (OAuth2 client credentials for the Schema Service)
- Quartz
- ConnectionStrings (MetaConnection, QuartzConnection)

Example (trim to your needs):

```
{
  "Kestrel": {
    "Endpoints": {
      "Http": { "Url": "http://+:5001" },
      "Https": { "Url": "https://+:6001" }
    }
  },
  "Authorization": {
    "Authority": "https://localhost:3001/realms/ballware",
    "Audience": "metaApi",
    "RequireHttpsMetadata": true,
    "TenantClaim": "tenant",
    "UserIdClaim": "sub",
    "RightClaim": "right",
    "RequiredMetaScope": "metaApi",
    "RequiredServiceScope": "serviceApi"
  },
  "Storage": {
    "Provider": "postgres",               // "postgres" or "mssql"
    "AutoMigrations": true,
    "SeedPath": "seed",
    "AutoSeedAdminTenant": false,
    "EnableCaching": true
  },
  "Cache": {
    "RedisConfiguration": "",             // empty => in-memory cache
    "RedisInstanceName": "ballware.meta:",
    "CacheExpirationHours": 3
  },
  "Cors": {
    "AllowedOrigins": "*",
    "AllowedMethods": "*",
    "AllowedHeaders": "*"
  },
  "Swagger": {
    "EnableClient": true,
    "ClientId": "ballwareweb",
    "RequiredScopes": "openid"
  },
  "StorageClient": {
    "ServiceUrl": "https://localhost:6005",
    "TokenEndpoint": "https://localhost:3001/realms/ballware/protocol/openid-connect/token",
    "ClientId": "storage",
    "ClientSecret": "<secret>",
    "Scopes": "openid serviceApi offline_access"
  },
  "SchemaClient": {
    "ServiceUrl": "https://localhost:6002",
    "TokenEndpoint": "https://localhost:3001/realms/ballware/protocol/openid-connect/token",
    "ClientId": "generic",
    "ClientSecret": "<secret>",
    "Scopes": "openid schemaApi offline_access"
  },
  "Quartz": {
    // Example for PostgreSQL-backed job store
    "quartz.scheduler.instanceName": "ballware-local-meta",
    "quartz.jobStore.dataSource": "ballware",
    "quartz.dataSource.ballware.provider": "Npgsql",
    "quartz.dataSource.ballware.connectionStringName": "QuartzConnection"

    // Use this for an in-memory job store in demos/tests:
    // "quartz.jobStore.type": "Quartz.Simpl.RAMJobStore, Quartz"
  },
  "ConnectionStrings": {
    "MetaConnection": "Host=localhost;Port=5432;Database=meta;Username=meta;Password=***",
    "QuartzConnection": "Host=localhost;Port=5432;Database=quartz;Username=quartz;Password=***"
  }
}
```

Notes:

- Storage.Provider: `postgres` or `mssql`; matching AddBallwareMetaStorageForPostgres/SqlServer is applied automatically.
- Cache: if `RedisConfiguration` is empty, an in-memory distributed cache is used; otherwise, Redis via StackExchange.Redis.
- Swagger: when `EnableClient` is true, the service exposes two Swagger docs: "meta" and "service" with OIDC security.
- Clients: outgoing HTTP calls to Storage and Schema services use OAuth2 client credentials; in Debug builds, server certificate validation is disabled for those outgoing calls only.


## Authentication and authorization

- The service uses JWT Bearer authentication.
- It validates tokens against `Authorization.Authority` and `Authorization.Audience`.
- Authorization is scope-based:
  - Policy `metaApi` requires the `Authorization.RequiredMetaScope` scope (default: `metaApi`).
  - Policy `serviceApi` requires the `Authorization.RequiredServiceScope` scope (default: `serviceApi`).
- Swagger UI is configured with an OpenID Connect security definition pointing to `Authority + /.well-known/openid-configuration`.

To use Swagger with auth:

1) Set `Authorization.Authority`, `Audience`, and `Swagger.ClientId` appropriately.
2) Open `https://localhost:6001/swagger` and pick either the Meta or Service API.
3) Authorize via the OIDC button; scopes are taken from `Swagger.RequiredScopes`.


## Databases and migrations

- Metadata DB: connection via `ConnectionStrings:MetaConnection`.
- Quartz DB (when not using RAMJobStore): connection via `ConnectionStrings:QuartzConnection` with appropriate Quartz configuration keys.
- If `Storage.AutoMigrations` is true, EF Core migrations are applied on startup.
- Optional seeding:
  - `Storage.SeedPath` defaults to `seed` (relative to the service working directory).
  - `Storage.AutoSeedAdminTenant` can bootstrap an admin tenant if supported by your setup.


## Caching

- Built-in caching options reside under `Cache`.
- `Cache.RedisConfiguration` empty → in-memory cache; otherwise, Redis is used.
- `Cache.CacheExpirationHours` controls default TTL of entries.


## Certificates and HTTPS (local development)

- There is no bundled certificate in this repository. Use the system "ASP.NET Core HTTPS development certificate".
- Run `dotnet dev-certs https --trust` once on your machine.
- Do not configure a certificate path in Kestrel for Development; Kestrel will select the trusted system dev certificate automatically.


## Troubleshooting

JWT validation error IDX10500: "Signature validation failed. No security keys were provided to validate the signature."

- Cause: The service could not download the issuer’s JWKS (public keys) from the OIDC discovery endpoint, often due to HTTPS certificate validation issues with a self-signed issuer certificate.
- Fix options:
  - Preferable: Use a trusted certificate for your OIDC provider (e.g., via mkcert/localhost certs) or import the issuer’s CA into your OS trust store (on macOS: Keychain Access → System → Certificates → import and mark as trusted).
  - Ensure `Authorization.Authority` correctly points to your realm (e.g., Keycloak `https://localhost:3001/realms/ballware`).
  - Verify network reachability from the service container/host to the issuer.
  - Note: There is no configuration switch in this service to bypass issuer certificate validation for JWT metadata download. Such a behavior would require a code change and should be restricted to Development only if ever used.

Swagger cannot authorize

- Make sure `Swagger.ClientId` exists in your IdP and `Swagger.RequiredScopes` match available scopes.
- Check CORS: allow the origin of your browser front-end if calling APIs directly.

Database connection issues

- Verify `ConnectionStrings:MetaConnection` and `Storage.Provider` (postgres/mssql) are consistent.
- For Quartz, ensure the chosen job store matches your provider settings or switch to RAMJobStore for local demos.


## Docker

For a full dockerized environment (including Keycloak, Storage, Schema), please refer to https://github.com/ballware/ballware-docker-compose. That setup handles certificates and inter-service wiring.

This repository also includes a service `Dockerfile` under `src/Ballware.Meta.Service/` if you need a custom build.


## Logging

- Serilog is configured via the `Serilog` section. Defaults log to console at Information level (Microsoft/System at Warning).
- Adjust levels and enrichers as needed in configuration.


## FAQ

Q: Can I disable validation of self-signed issuer certificates via configuration only?

- A: No. There is no configuration key to do this for JWT (OIDC) backchannel requests. You must trust the issuer certificate on the OS or change code in Development to relax validation (not recommended for shared branches).

Q: Can I reference the dotnet-issued development certificate directly instead of a duplicated PFX?

- A: Yes. For local runs, do not specify a certificate path in Kestrel and run `dotnet dev-certs https --trust`. Kestrel will use the system dev certificate automatically in Development.


## Development

- Run tests: `dotnet test`
- Typical inner loop: `dotnet run --project src/Ballware.Meta.Service`


## License

MIT License. See `LICENSE` in this repository.

Copyright (c) 2025 ballware Software & Consulting, Frank Ballmeyer (https://www.ballware.de)
