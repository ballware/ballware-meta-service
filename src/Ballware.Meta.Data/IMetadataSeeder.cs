namespace Ballware.Meta.Data;

public interface IMetadataSeeder
{
    Task<Guid?> GetAdminTenantIdAsync();
    
    Task<Guid?> SeedAdminTenantAsync();
    Task SeedCustomerTenantAsync(Guid tenantId, string name);
}