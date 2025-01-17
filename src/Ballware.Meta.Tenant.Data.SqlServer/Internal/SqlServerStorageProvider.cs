using System.Data;
using Microsoft.Data.SqlClient;

namespace Ballware.Meta.Tenant.Data.SqlServer.Internal;

class SqlServerStorageProvider : ITenantStorageProvider
{   
    public string GetConnectionString(Meta.Data.Public.Tenant tenant)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = tenant.Server,
            InitialCatalog = tenant.Catalog,
            UserID = tenant.User,
            Password = tenant.Password,
            Encrypt = true,
            PersistSecurityInfo = false,
            IntegratedSecurity = false,
        };
        
        return connectionStringBuilder.ConnectionString;
    }

    public IDbConnection OpenConnection(Meta.Data.Public.Tenant tenant)
    {
        var connection = new SqlConnection(GetConnectionString(tenant));
        
        connection.Open();
        
        return connection;
    }

    public async Task<IDbConnection> OpenConnectionAsync(Meta.Data.Public.Tenant tenant)
    {
        var connection = new SqlConnection(GetConnectionString(tenant));
        
        await connection.OpenAsync();
        
        return connection;
    }

    public string ApplyTenantPlaceholder(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options)
    {
        if (!string.IsNullOrEmpty(source))
        {
            source = source.Replace("[ballwareschema]", tenant.Schema ?? "dbo");
        }

        if (!string.IsNullOrEmpty(source) && options.ReplaceTenantId)
        {
            source = source.Replace("@tenantId", $"'{tenant.Id}'");
        }

        if (!string.IsNullOrEmpty(source) && options.ReplaceClaims)
        {
            source = source.Replace("@claims", "''");
        }

        return source;
    }

    public Task<string> ApplyTenantPlaceholderAsync(Meta.Data.Public.Tenant tenant, string source, TenantPlaceholderOptions options)
    {
        return Task.FromResult(ApplyTenantPlaceholder(tenant, source, options));
    }
}