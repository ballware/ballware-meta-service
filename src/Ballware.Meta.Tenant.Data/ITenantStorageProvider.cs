using System.Data;

namespace Ballware.Meta.Tenant.Data;

public interface ITenantStorageProvider
{
    string GetConnectionString(Meta.Data.Public.Tenant tenant);

    IDbConnection OpenConnection(Meta.Data.Public.Tenant tenant);
    Task<IDbConnection> OpenConnectionAsync(Meta.Data.Public.Tenant tenant);

    string ApplyTenantPlaceholder(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options);
    Task<string> ApplyTenantPlaceholderAsync(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options);
}