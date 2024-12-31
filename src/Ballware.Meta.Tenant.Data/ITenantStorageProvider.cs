using System.Data;

namespace Ballware.Meta.Tenant.Data;

public interface ITenantStorageProvider
{
    string GetConnectionString(Meta.Data.Tenant tenant);
    
    IDbConnection OpenConnection(Meta.Data.Tenant tenant);
    Task<IDbConnection> OpenConnectionAsync(Meta.Data.Tenant tenant);
    
    string ApplyTenantPlaceholder(Meta.Data.Tenant tenant, string source, TenantPlaceholderOptions options);
    Task<string> ApplyTenantPlaceholderAsync(Meta.Data.Tenant tenant, string source, TenantPlaceholderOptions options);
}