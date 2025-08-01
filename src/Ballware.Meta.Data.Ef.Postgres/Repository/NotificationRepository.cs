using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

public class NotificationRepository : NotificationBaseRepository
{
    public NotificationRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Notification, Persistables.Notification>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", name as \"Name\" FROM notification WHERE tenant_id='{tenantId}'");
    }
}
