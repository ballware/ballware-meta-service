using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class DocumentRepository : DocumentBaseRepository
{
    public DocumentRepository(IMapper mapper, IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.Document, Persistables.Document>? hook = null)
        : base(mapper, dbContext, hook)
    {
    }

    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult(
            $"select Uuid as Id, DisplayName as Name, State from Document where TenantId='{tenantId}'");
    }
}

