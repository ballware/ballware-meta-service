using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class MlModelMetaRepository : IMlModelMetaRepository
{
    private MetaDbContext DbContext { get; }

    public MlModelMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<MlModel?> MetadataByTenantAndIdAsync(Tenant tenant, Guid id)
    {
        return await DbContext.MlModels.SingleOrDefaultAsync(d => d.TenantId == tenant.Uuid && d.Uuid == id);
    }
    
    public virtual async Task<MlModel?> MetadataByTenantAndIdentifierAsync(Tenant tenant, string identifier) {
        
        return await DbContext.MlModels.SingleOrDefaultAsync(d => d.TenantId == tenant.Uuid && d.Identifier == identifier);
    }

    public virtual async Task SaveTrainingStateAsync(Tenant tenant, Guid userId, MlModelTrainingState state)
    {
        var model = await DbContext.MlModels.SingleOrDefaultAsync(j => j.TenantId == tenant.Uuid && j.Uuid == state.Id);

        if (model == null)
        {
            throw new Exception("Model not found");
        }
        
        model.TrainState = state.State;
        model.TrainResult = state.Result;
        model.LastChangerId = userId;
        model.LastChangeStamp = DateTime.Now;
        
        DbContext.Update(model);
        await DbContext.SaveChangesAsync();
    }
}