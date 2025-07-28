using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class SubscriptionRepository : SubscriptionBaseRepository
{
    public SubscriptionRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Subscription, Persistables.Subscription>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, NotificationId, UserId, Active from Subscription where TenantId='{tenantId}'");
    }
}