using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class SubscriptionRepository : SubscriptionBaseRepository
{
    public SubscriptionRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Subscription, Persistables.Subscription>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", notification_id AS \"NotificationId\", user_id AS \"UserId\", active AS \"Active\" FROM subscription WHERE tenant_id='{tenantId}'");
    }
}
