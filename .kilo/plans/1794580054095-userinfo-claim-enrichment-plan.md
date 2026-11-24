# Userinfo-Anreicherung im Ballware.Meta.Service

## Ziel
Claims sollen nicht mehr groß im Access Token stehen, sondern im Meta Service zur Laufzeit aus dem IdP-`userinfo`-Endpoint in den `ClaimsPrincipal` übernommen werden, wenn ein konfigurierter Scope (z. B. `metaApi`) vorliegt.

## Getroffene Entscheidungen
- Implementierung nur in `Ballware.Meta.Service`.
- Erweiterung über `JwtBearerOptions.Events.OnTokenValidated`.
- Anreicherung nur bei Positivliste konfigurierter Scopes, z. B. `metaApi`.
- `serviceApi` wird nicht angereichert, außer explizit in der Positivliste.
- Wenn ein konfiguriertem Userinfo-Scope vorliegt und `userinfo` fehlschlägt: fail closed und Authentifizierung ablehnen.
- Userinfo-Claims werden nur ergänzt, nicht ersetzt.
- Identische Duplikate (Claim-Typ + Claim-Wert) werden übersprungen.
- Keine Allow-Liste für Fach-Claims: alle nicht geschützten Userinfo-Claims werden ergänzt.
- OAuth/JWT-Struktur-Claims werden nicht ergänzt, um Token-/Scope-Semantik nicht zu verfälschen.
- `sub` aus Userinfo muss zum Token-`sub` passen; `sub` wird nicht ersetzt.

## Geschützte Claims
Nicht ergänzen, auch wenn Userinfo sie liefert:
- `iss`
- `aud`
- `azp`
- `sub`
- `scope`
- `scp`
- `nbf`
- `exp`
- `iat`
- `jti`
- `nonce`

## Architektur
```text
Request mit Bearer Token
  -> JwtBearer validiert Token
  -> OnTokenValidated
  -> Ist ein konfigurierter Userinfo-Scope im Token?
      -> nein: nicht anreichern
      -> ja: userinfo abrufen
          -> Erfolg: Claims ergänzen und Principal ersetzen
          -> Fehler: context.Fail() => 401
```

## Implementierungsaufgaben

### 1. Konfiguration hinzufügen
Neue Optionenklasse:
- `src/Ballware.Meta.Service/Configuration/UserInfoEnrichmentOptions.cs`

Beispielkonfiguration:
```json
{
  "Authorization": {
    "RequiredMetaScope": "metaApi",
    "RequiredServiceScope": "serviceApi",
    "UserInfo": {
      "Enabled": true,
      "Scopes": ["metaApi"],
      "ScopeClaimType": "scope",
      "CacheTtlSeconds": 60,
      "NegativeCacheTtlSeconds": 10,
      "TimeoutSeconds": 5,
      "MaxResponseBytes": 262144,
      "MaxClaims": 1000
    }
  }
}
```

Regeln:
- `Enabled` default `false`, damit rollout konfigurierbar ist.
- Wenn `Enabled=true` und `Scopes` leer ist: Konfigurationsfehler beim Start.
- `Scopes` sind Positivliste, exakte Scope-Namen, ordinal comparison.
- Optional `UserinfoEndpoint` konfigurierbar. Wenn nicht gesetzt: Discovery über `Authorization.Authority + "/.well-known/openid-configuration"`.

### 2. Enricher-Dienst erstellen
Dateien:
- `src/Ballware.Meta.Service/Authentication/UserInfoEnricher.cs`
- `src/Ballware.Meta.Service/Authentication/IUserInfoEnricher.cs`
- `src/Ballware.Meta.Service/Authentication/UserInfoEnrichmentResult.cs`

Interface:
```csharp
public interface IUserInfoEnricher
{
    Task<UserInfoEnrichmentResult> EnrichAsync(
        ClaimsPrincipal principal,
        string accessToken,
        CancellationToken cancellationToken);
}
```

Result:
```csharp
public enum UserInfoEnrichmentStatus
{
    NotEnriched,
    Enriched,
    Failed
}
```

Enricher-Verhalten:
1. Wenn `Enabled=false`: `NotEnriched`.
2. Scope aus Principal extrahieren:
   - Claim-Typ default `scope`.
   - Werte mit `Split(' ', StringSplitOptions.RemoveEmptyEntries)`.
   - Optional auch `scp`, falls tokens andere Mapping-Varianten verwenden.
3. Wenn kein konfigurierter Scope getroffen wird: `NotEnriched`.
4. `sub` und `azp` aus Token lesen.
   - `sub` fehlt: `Failed`.
   - `azp` fehlt: Cache-Key mit `"unknown"` verwenden.
5. Userinfo-Endpoint bestimmen:
   - konfigurierter `UserinfoEndpoint`, sonst OIDC-Metadata-Discovery.
6. GET `userinfo` mit `Authorization: Bearer <accessToken>`.
7. HTTP-Regeln:
   - kein Auto-Redirect
   - Timeout konfigurierbar
   - max response bytes
   - max claim count
8. JSON parsen:
   - Top-Level muss JSON-Objekt sein.
   - `sub` aus Userinfo muss gleich Token-`sub` sein, sonst `Failed`.
   - Fach-Claims werden ergänzt.
   - Geschützte Claims werden ignoriert.
   - Scalar string/number/bool: als `Claim` ergänzen.
   - Array von Scalars: mehrere `Claim`-Einträge erzeugen.
   - Object/Array-of-Objects: JSON-Rohstring als `Claim` ergänzen.
   - Identische Duplikate `Claim.Type + Claim.Value` überspringen.
9. Neuen `ClaimsPrincipal` bauen:
   - alle Original-Claims behalten
   - ergänzte Userinfo-Claims hinzufügen
   - Original-`AuthenticationType`, `NameClaimType`, `RoleClaimType` beibehalten
10. Cache verwenden:
   - Cache-Key: `userinfo:{issuerOrAuthority}:{azp}:{sub}`
   - erfolgreiche Claims für `CacheTtlSeconds` cachen
   - Fehler kurz für `NegativeCacheTtlSeconds` cachen
   - single-flight pro Instance, um parallele IdP-Calls für denselben Key zu vermeiden

### 3. DI und JwtBearer-Event verdrahten
In `src/Ballware.Meta.Service/Startup.cs`:
- `AddOptionsWithValidateOnStart<UserInfoEnrichmentOptions>()` für `Authorization:UserInfo` registrieren.
- `AddHttpClient` mit `AllowAutoRedirect=false` registrieren.
- `IUserInfoEnricher` singleton registrieren.
- `OnMessageReceived` behalten.
- `OnTokenValidated` ergänzen.

Event-Logik:
```csharp
OnTokenValidated = async context =>
{
    var enricher = context.HttpContext.RequestServices.GetRequiredService<IUserInfoEnricher>();

    var authorizationHeader = context.Request.Headers.Authorization.ToString();
    var accessToken = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        ? authorizationHeader["Bearer ".Length..].Trim()
        : string.Empty;

    if (context.Principal == null)
    {
        return;
    }

    var result = await enricher.EnrichAsync(
        context.Principal,
        accessToken,
        context.HttpContext.RequestAborted);

    switch (result.Status)
    {
        case UserInfoEnrichmentStatus.NotEnriched:
            return;
        case UserInfoEnrichmentStatus.Failed:
            context.Fail();
            return;
        case UserInfoEnrichmentStatus.Enriched:
            if (result.Principal != null)
            {
                context.Principal = result.Principal;
            }
            return;
    }
};
```

### 4. Tests hinzufügen
Neues Testprojekt:
- `test/Ballware.Meta.Service.Tests/Ballware.Meta.Service.Tests.csproj`
- zum `ballware-meta-service.sln` hinzufügen

Mögliche Unit-Tests:
- `Enabled=false`: kein HTTP-Aufruf, `NotEnriched`.
- Token ohne `metaApi`: kein HTTP-Aufruf, `NotEnriched`.
- Token mit `metaApi`: Userinfo wird aufgerufen und Fach-Claims ergänzt.
- Token mit `serviceApi`, aber `Scopes=["metaApi"]`: keine Anreicherung.
- Token mit `metaApi serviceApi`: Anreicherung, weil Positivliste `metaApi` enthält.
- Userinfo gibt Fach-Claims ohne Allow-Liste zurück: alle werden ergänzt.
- Identische Claims werden nicht dupliziert.
- Geschützte Claims wie `scope`, `aud`, `azp`, `sub` werden nicht ergänzt.
- Userinfo-`sub` != Token-`sub`: `Failed`.
- HTTP-Fehler bei konfiguriertem Scope: `Failed`.
- Cache-Treffer ruft Userinfo nur einmal auf.
- Negative Cache verhindert Immediate-Retry.
- Arrays aus Userinfo werden als mehrere Claims ergänzt.
- komplexe JSON-Werte werden als JSON-String ergänzt.

### 5. Keycloak-Mapper anpassen
Nachdem Enricher lokal und im Test läuft:
- `ballwareweb`-Client-Mappers anpassen:
  - `Rights` (`ballware-token-mapper`): `access.token.claim=false`, `userinfo.token.claim=true` behalten.
  - `Tenant`: empfohlen `access.token.claim=false`, `userinfo.token.claim=true`.
  - `Allowed Tenant`: empfohlen `access.token.claim=false`, `userinfo.token.claim=true`.
  - `Person`: empfohlen `access.token.claim=false`, `userinfo.token.claim=true`.
- `Audience`-Mapper muss `access.token.claim=true` behalten, damit Meta Service Audience prüft.
- Anzuwendende Dateien oder live Keycloak config:
  - `/Users/frank/Documents/GitHub/ballware-keycloak/import/ballware-realm.json`
  - `/Users/frank/Documents/GitHub/ballware-keycloak/keycloak-export/ballware-realm.json`
  - `/Users/frank/Documents/GitHub/ballware-docker-compose/realms/ballware-realm.tpl`
  - relevante live Keycloak realm config

Wichtig:
- Wenn `access.token.claim=true` bleibt und Userinfo dieselben Fach-Claims zurückgibt, entstehen doppelte Claims im Principal.
- Für das Token-Ziel müssen die großen `right`/`tenant`-Claims aus dem Access Token herausgenommen werden.

## Validierung
1. Build:
   - `dotnet build ballware-meta-service.sln`
2. Tests:
   - `dotnet test ballware-meta-service.sln`
3. Manueller Keycloak-Test:
   - `ballwareweb`-Login mit `metaApi`-Scope.
   - Access-Token auf Audience und `scope` prüfen.
   - Meta-Endpoint mit Token aufrufen.
   - Log prüfen, dass `userinfo` nur bei passendem Scope aufgerufen wird.
   - prüfen, dass `tenant`/`right` im Principal vorhanden sind, obwohl sie nicht mehr im Access Token stehen.
4. Service-Token-Test:
   - `serviceApi`-Token verwenden.
   - prüfen, dass kein `userinfo`-Call ausgelöst wird.
5. MCP-Test:
   - MCP-Endpoint mit `metaApi`-Token aufrufen.
   - prüfen, dass Tools Zugriff auf `tenant`/`right`-Claims über `IPrincipalUtils` haben.
6. Token-Size-Messung:
   - Access Token vor/nach Keycloak-Mapper-Änderung vergleichen.

## Risiken
- Append-only kann doppelte Fach-Claims erzeugen, wenn Access Token noch `tenant`/`right` enthält.
- `DefaultPrincipalUtils.GetUserTenandId` verwendet den ersten `tenant`-Claim; doppelte tenant-Werte können problematisch sein.
- `DefaultPrincipalUtils.GetUserRights` kombiniert alle `right`-Claims; doppelte alte und neue `right`-Claims können Rechte erweitern.
- Keycloak-`userinfo` kann `openid`-Scope oder Audience-Voraussetzungen benötigen. Das muss mit einem echten `ballwareweb`-Token validiert werden.
- MCP-Handler muss nachweislich denselben `JwtBearer`-Enrichment-Pfad nutzen oder das Principal mit `OnTokenValidated`-Ergebnis verwenden.

## Reihenfolge für Rollout
1. Enricher implementieren, aber `Authorization:UserInfo:Enabled=false` lassen.
2. Tests lokal mit aktiviertem Enricher und unveränderten Keycloak-Mappern durchführen.
3. In Dev `Enabled=true` setzen und mit `access.token.claim=true` testen.
4. Danach Keycloak-Mapper `access.token.claim=false` für die per Userinfo angereicherten Claims setzen.
5. Token-Size, Meta-Endpoints, MCP und Service-Tokens erneut validieren.
