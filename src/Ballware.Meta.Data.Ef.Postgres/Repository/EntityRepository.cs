using AutoMapper;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Public;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Postgres.Repository;

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
        return Task.FromResult($"SELECT uuid AS \"Id\", entity as \"Entity\", display_name AS \"Name\" FROM entity WHERE tenant_id='{tenantId}'");
    }
    
    public override Task<string> GenerateRightsListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"SELECT uuid AS \"Id\", identifier as \"Identifier\", display_name AS \"Name\", container as \"Container\" FROM entity_right WHERE tenant_id='{tenantId}' ORDER BY container, identifier");
    }
}
