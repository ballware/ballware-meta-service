using Ballware.Meta.Data;

namespace Ballware.Meta.Authorization;

public interface ITenantRightsChecker
{
    public Task<bool> HasRightAsync(Tenant tenant, string application, string entity, Dictionary<string, object> claims, string right);
    
}