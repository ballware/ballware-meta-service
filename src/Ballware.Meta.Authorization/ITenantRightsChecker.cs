using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Authorization;

public interface ITenantRightsChecker
{
    public Task<bool> HasRightAsync(Tenant tenant, string application, string entity, IDictionary<string, object> claims, string right);

}