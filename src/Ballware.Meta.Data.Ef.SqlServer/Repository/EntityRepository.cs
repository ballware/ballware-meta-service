using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Public;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.SqlServer.Repository;

public class EntityRepository : EntityBaseRepository
{
    public EntityRepository(IMapper mapper, 
        IProcessingStateMetaRepository processingStateMetaRepository,
        IPickvalueMetaRepository pickvalueMetaRepository,
        IEntityRightMetaRepository entityRightMetaRepository,
        ICharacteristicAssociationMetaRepository characteristicAssociationMetaRepository,
        IMetaDbContext dbContext,
        ITenantableRepositoryHook<Public.EntityMetadata, Persistables.EntityMetadata>? hook = null)
        : base(mapper, processingStateMetaRepository, pickvalueMetaRepository, entityRightMetaRepository, characteristicAssociationMetaRepository, dbContext, hook)
    {
    }
    
    public override Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Entity, DisplayName as Name from Entity where TenantId='{tenantId}'");
    }
    
    public override Task<string> GenerateRightsListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Identifier, DisplayName as Name, Container from EntityRight where TenantId='{tenantId}' order by Container, Identifier");
    }
}