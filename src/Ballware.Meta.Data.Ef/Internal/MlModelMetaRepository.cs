using AutoMapper;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class MlModelMetaRepository : TenantableBaseRepository<Public.MlModel, Persistables.MlModel>, IMlModelMetaRepository
{
    public MlModelMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.MlModel, Persistables.MlModel>? hook = null) 
        : base(mapper, dbContext, hook) { }

    public virtual async Task<Public.MlModel?> MetadataByTenantAndIdAsync(Public.Tenant tenant, Guid id)
    {
        var result = await Context.MlModels.SingleOrDefaultAsync(d => d.TenantId == tenant.Id && d.Uuid == id);

        return result != null ? Mapper.Map<Public.MlModel>(result) : null;
    }

    public virtual async Task<Public.MlModel?> MetadataByTenantAndIdentifierAsync(Public.Tenant tenant, string identifier)
    {

        var result = await Context.MlModels.SingleOrDefaultAsync(d => d.TenantId == tenant.Id && d.Identifier == identifier);

        return result != null ? Mapper.Map<Public.MlModel>(result) : null;
    }

    public virtual async Task SaveTrainingStateAsync(Public.Tenant tenant, Guid userId, MlModelTrainingState state)
    {
        var model = await Context.MlModels.SingleOrDefaultAsync(j => j.TenantId == tenant.Id && j.Uuid == state.Id);

        if (model == null)
        {
            throw new Exception("Model not found");
        }

        model.TrainState = state.State;
        model.TrainResult = state.Result;
        model.LastChangerId = userId;
        model.LastChangeStamp = DateTime.Now;

        Context.Update(model);

        await Context.SaveChangesAsync();
    }
    
    public virtual async Task<IEnumerable<MlModelSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.MlModels.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Identifier)
            .Select(r => new MlModelSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier }));
    }
    
    public virtual async Task<MlModelSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.MlModels.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(r => new MlModelSelectListEntry
                { Id = r.Uuid, Identifier = r.Identifier })
            .FirstOrDefaultAsync();
    }
}