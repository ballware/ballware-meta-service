using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Authorization;

public interface IEntityRightsChecker
{
    public Task<bool> HasRightAsync(Guid tenantId, EntityMetadata metadata, Dictionary<string, object> claims, string right, IDictionary<string, object> param, bool tenantResult);
}