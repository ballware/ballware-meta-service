using System.Data;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Tenant.Data.Internal;

class TenantLookupProviderProxy : ITenantLookupProvider
{
    private IProviderRegistry ProviderRegistry { get; }

    public TenantLookupProviderProxy(IProviderRegistry providerRegistry)
    {
        ProviderRegistry = providerRegistry;
    }

    public async Task<IEnumerable<dynamic>> SelectListForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.SelectListForLookupAsync(tenant, lookup, rights);
    }

    public async Task<dynamic> SelectByIdForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, object id, IEnumerable<string> rights)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.SelectByIdForLookupAsync(tenant, lookup, id, rights);
    }

    public async Task<IEnumerable<dynamic>> SelectListForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.SelectListForLookupWithParamAsync(tenant, lookup, rights, param);
    }

    public async Task<dynamic> SelectByIdForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param, object id)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.SelectByIdForLookupWithParamAsync(tenant, lookup, rights, param, id);
    }

    public async Task<IEnumerable<string>> AutoCompleteForLookupAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.AutoCompleteForLookupAsync(tenant, lookup, rights);
    }

    public async Task<IEnumerable<string>> AutoCompleteForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Lookup lookup, IEnumerable<string> rights, string param)
    {
        var provider = ProviderRegistry.GetLookupProvider(tenant.Provider ?? "mssql");

        return await provider.AutoCompleteForLookupWithParamAsync(tenant, lookup, rights, param);
    }
}