using System.Data;
using System.Data.Common;
using AutoMapper;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class EntityMetaRepository : TenantableBaseRepository<Public.EntityMetadata, Persistables.EntityMetadata>, IEntityMetaRepository
{
    private IProcessingStateMetaRepository ProcessingStateMetaRepository { get; }
    private IPickvalueMetaRepository PickvalueMetaRepository { get; }
    private IEntityRightMetaRepository EntityRightMetaRepository { get; }
    private ICharacteristicAssociationMetaRepository CharacteristicAssociationMetaRepository { get; }

    public EntityMetaRepository(IMapper mapper, 
        IProcessingStateMetaRepository processingStateMetaRepository,
        IPickvalueMetaRepository pickvalueMetaRepository,
        IEntityRightMetaRepository entityRightMetaRepository,
        ICharacteristicAssociationMetaRepository characteristicAssociationMetaRepository,
        MetaDbContext dbContext,
        ITenantableRepositoryHook<Public.EntityMetadata, Persistables.EntityMetadata>? hook = null)
        : base(mapper, dbContext, hook)
    {
        ProcessingStateMetaRepository = processingStateMetaRepository;
        PickvalueMetaRepository = pickvalueMetaRepository;
        EntityRightMetaRepository = entityRightMetaRepository;
        CharacteristicAssociationMetaRepository = characteristicAssociationMetaRepository;
    }

    protected override async Task<EntityMetadata> ExtendByIdAsync(string identifier, IDictionary<string, object> claims, Guid tenantId, EntityMetadata value)
    {
        var result = await base.ExtendByIdAsync(identifier, claims, tenantId, value);

        if (result.Entity != null && "exportjson".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            result.States = await ProcessingStateMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", result.Entity }
            });
            
            result.Pickvalues = await PickvalueMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", result.Entity }
            });
            
            result.Rights = await EntityRightMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", result.Entity }
            });
            
            result.CharacteristicAssociations = await CharacteristicAssociationMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", result.Entity }
            });
        }
        
        return result;
    }

    protected override async Task AfterSaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, EntityMetadata value,
        Persistables.EntityMetadata persistable, bool insert)
    {
        if (value.Entity != null && "importjson".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            await MergeChildCollectionsAsync(tenantId, userId, claims, value.Entity, value.States,
                ProcessingStateMetaRepository);
            
            await MergeChildCollectionsAsync(tenantId, userId, claims, value.Entity, value.Pickvalues,
                PickvalueMetaRepository);
            
            await MergeChildCollectionsAsync(tenantId, userId, claims, value.Entity, value.Rights,
                EntityRightMetaRepository);
            
            await MergeChildCollectionsAsync(tenantId, userId, claims, value.Entity, value.CharacteristicAssociations,
                CharacteristicAssociationMetaRepository);
        }
        
        await base.AfterSaveAsync(tenantId, userId, identifier, claims, value, persistable, insert);
    }

    protected override async Task BeforeRemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims, Persistables.EntityMetadata persistable)
    {
        await base.BeforeRemoveAsync(tenantId, userId, claims, persistable);
        
        if (persistable.Entity != null)
        {
            var processingStates = await ProcessingStateMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", persistable.Entity }
            });

            foreach (var processingState in processingStates)
            {
                await ProcessingStateMetaRepository.RemoveAsync(tenantId, userId, claims, new Dictionary<string, object>
                {
                    { "Id", processingState.Id }
                });
            }
             
            var pickvalues = await PickvalueMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", persistable.Entity }
            });
                
            foreach (var pickvalue in pickvalues)
            {
                await PickvalueMetaRepository.RemoveAsync(tenantId, userId, claims, new Dictionary<string, object>
                {
                    { "Id", pickvalue.Id }
                });
            }    
            
            var entityrights = await EntityRightMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", persistable.Entity }
            });
                
            foreach (var entityright in entityrights)
            {
                await EntityRightMetaRepository.RemoveAsync(tenantId, userId, claims, new Dictionary<string, object>
                {
                    { "Id", entityright.Id }
                });
            } 
            
            var characteristics = await CharacteristicAssociationMetaRepository.QueryAsync(tenantId, "entity", claims, new Dictionary<string, object>
            {
                { "entity", persistable.Entity }
            });
                
            foreach (var characteristic in characteristics)
            {
                await CharacteristicAssociationMetaRepository.RemoveAsync(tenantId, userId, claims, new Dictionary<string, object>
                {
                    { "Id", characteristic.Id }
                });
            }
        }
    }

    private static async Task MergeChildCollectionsAsync<TEditable>(Guid tenantId, Guid? userId, IDictionary<string, object> claims, string entity, IEnumerable<TEditable> nextChildren,
        ITenantableRepository<TEditable> repository) where TEditable : class, IEditable
    {
        var existingChildren = (await repository.QueryAsync(tenantId, "entity", claims,
            new Dictionary<string, object>
            {
                { "entity", entity }
            })).ToList();
            
        foreach (var droppedChild in existingChildren.Where(s => !nextChildren.Any(v => v.Id == s.Id)))
        {
            await repository.RemoveAsync(tenantId, userId, claims, new Dictionary<string, object>
            {
                { "Id", droppedChild.Id }
            });
        }
            
        foreach (var addedOrUpdatedChild in nextChildren)
        {
            await repository.SaveAsync(tenantId, userId, "importjson", claims, addedOrUpdatedChild);
        }
    }

    public virtual async Task<Public.EntityMetadata?> ByIdAsync(Guid tenantId, Guid id)
    {
        var result = await Context.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Uuid == id);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public virtual async Task<Public.EntityMetadata?> ByEntityAsync(Guid tenantId, string entity)
    {
        var result = await Context.Entities.SingleOrDefaultAsync(e => e.TenantId == tenantId && e.Entity == entity);

        return result != null ? Mapper.Map<Public.EntityMetadata>(result) : null;
    }

    public virtual async Task<IEnumerable<EntityRightSelectListEntry>> SelectListEntityRightsForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.EntityRights.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Container).ThenBy(r => r.Identifier)
            .Select(r => new EntityRightSelectListEntry
            { Id = r.Uuid, Identifier = r.Identifier, Name = r.DisplayName, Container = r.Container }));
    }
    
    public virtual async Task<IEnumerable<EntitySelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.Entities.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Entity)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName }));
    }
    
    public virtual async Task<EntitySelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.Entities.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName })
            .FirstOrDefaultAsync();
    }
    
    public virtual async Task<EntitySelectListEntry?> SelectByIdentifierForTenantAsync(Guid tenantId, string identifier)
    {
        return await Context.Entities.Where(r => r.TenantId == tenantId && r.Entity == identifier)
            .Select(r => new EntitySelectListEntry
                { Id = r.Uuid, Entity = r.Entity, Name = r.DisplayName })
            .FirstOrDefaultAsync();
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Entity, DisplayName as Name from Entity where TenantId='{tenantId}'");
    }
    
    public Task<string> GenerateRightsListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Uuid as Id, Identifier, DisplayName as Name, Container from EntityRight where TenantId='{tenantId}' order by Container, Identifier");
    }
}