using Ballware.Meta.Data.Public;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Ballware.Meta.Tenant.Data.SqlServer.Internal;

class SqlServerLookupProvider : ITenantLookupProvider
{
    private ITenantStorageProvider StorageProvider { get; }

    public SqlServerLookupProvider(ITenantStorageProvider storageProvider)
    {
        StorageProvider = storageProvider;
    }

    public async Task<IEnumerable<dynamic>> SelectListForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights)
    {
        if (string.IsNullOrEmpty(lookup.ListQuery))
        {
            return Array.Empty<dynamic>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ListQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QueryAsync(sql, new { tenantId = tenant.Id, claims = string.Join(",", rights) });
    }

    public async Task<dynamic> SelectByIdForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, object id, IEnumerable<string> rights)
    {
        if (string.IsNullOrEmpty(lookup.ByIdQuery))
        {
            return Array.Empty<dynamic>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ByIdQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QuerySingleAsync(sql, new { id, tenantId = tenant.Id, claims = string.Join(",", rights) });
    }

    public async Task<IEnumerable<dynamic>> SelectListForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param)
    {
        if (string.IsNullOrEmpty(lookup.ListQuery))
        {
            return Array.Empty<dynamic>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ListQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QueryAsync(sql, new { tenantId = tenant.Id, claims = string.Join(",", rights), param });
    }

    public async Task<dynamic> SelectByIdForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param, object id)
    {
        if (string.IsNullOrEmpty(lookup.ByIdQuery))
        {
            return Array.Empty<dynamic>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ByIdQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QuerySingleAsync(sql, new { id, tenantId = tenant.Id, claims = string.Join(",", rights), param });
    }

    public async Task<IEnumerable<string>> AutoCompleteForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights)
    {
        if (string.IsNullOrEmpty(lookup.ListQuery))
        {
            return Array.Empty<string>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ListQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QueryAsync<string>(sql, new { tenantId = tenant.Id, claims = string.Join(",", rights) });
    }

    public async Task<IEnumerable<string>> AutoCompleteForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param)
    {
        if (string.IsNullOrEmpty(lookup.ListQuery))
        {
            return Array.Empty<string>();
        }

        await using var connection = new SqlConnection(Utils.GetConnectionString(tenant));

        var sql = await StorageProvider.ApplyTenantPlaceholderAsync(tenant, lookup.ListQuery,
            TenantPlaceholderOptions.Create());

        return await connection.QueryAsync<string>(sql, new { tenantId = tenant.Id, claims = string.Join(",", rights), param });
    }
}