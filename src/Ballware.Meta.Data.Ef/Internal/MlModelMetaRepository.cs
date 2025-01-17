using AutoMapper;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class MlModelMetaRepository : TenantableBaseRepository<Public.MlModel, Persistables.MlModel>, IMlModelMetaRepository
{
    public MlModelMetaRepository(IMapper mapper, MetaDbContext dbContext) : base(mapper, dbContext) { }

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
}