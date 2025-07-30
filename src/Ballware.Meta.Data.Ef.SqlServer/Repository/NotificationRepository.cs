using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class NotificationRepository : NotificationBaseRepository
{
    public NotificationRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Notification, Persistables.Notification>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Name from Notification where TenantId='{tenantId}'");
    }
}
