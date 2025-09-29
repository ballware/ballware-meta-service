using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Data;

public interface IMetadataSeeder
{
    Task<Guid?> GetAdminTenantIdAsync();

    Task<Guid?> SeedAdminTenantAsync(Tenant? tenant = null);
    Task SeedCustomerTenantAsync(Tenant tenant, Guid userId);
}