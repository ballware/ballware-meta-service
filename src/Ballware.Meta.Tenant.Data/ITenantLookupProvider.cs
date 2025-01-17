namespace Ballware.Meta.Tenant.Data;

public interface ITenantLookupProvider
{
    Task<IEnumerable<dynamic>> SelectListForLookupAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup,
        IEnumerable<string> rights);

    Task<dynamic> SelectByIdForLookupAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup, object id, IEnumerable<string> rights);

    Task<IEnumerable<dynamic>> SelectListForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup, IEnumerable<string> rights,
        string param);

    Task<dynamic> SelectByIdForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup,
        IEnumerable<string> rights, string param, object id);

    Task<IEnumerable<string>> AutoCompleteForLookupAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup,
        IEnumerable<string> rights);

    Task<IEnumerable<string>> AutoCompleteForLookupWithParamAsync(Meta.Data.Public.Tenant tenant, Meta.Data.Public.Lookup lookup,
        IEnumerable<string> rights, string param);

}