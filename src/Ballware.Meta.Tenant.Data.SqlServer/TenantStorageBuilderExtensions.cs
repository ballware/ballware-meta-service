using Ballware.Meta.Tenant.Data.SqlServer.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Tenant.Data.SqlServer;

public static class TenantStorageBuilderExtensions
{
    public static TenantStorageBuilder AddSqlServerTenantDataStorage(this TenantStorageBuilder builder)
    {
        var storageProvider = new SqlServerStorageProvider();

        builder.ProviderRegistry.RegisterStorageProvider("mssql", storageProvider);
        builder.ProviderRegistry.RegisterLookupProvider("mssql", new SqlServerLookupProvider(storageProvider));

        return builder;
    }
}