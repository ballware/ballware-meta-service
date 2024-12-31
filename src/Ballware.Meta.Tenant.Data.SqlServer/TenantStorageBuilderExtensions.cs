using Ballware.Meta.Tenant.Data.SqlServer.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data.SqlServer;

public static class TenantStorageBuilderExtensions
{
    public static TenantStorageBuilder AddSqlServerTenantDataStorage(this TenantStorageBuilder builder)
    {
        builder.ProviderRegistry.RegisterStorageProvider("mssql", new SqlServerStorageProvider());
        
        return builder;
    }
}