using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Authorization;

public interface IEntityRightsChecker
{
    public Task<bool> HasRightAsync(Guid tenantId, EntityMetadata metadata, IDictionary<string, object> claims, string right, object param, bool tenantResult);
}