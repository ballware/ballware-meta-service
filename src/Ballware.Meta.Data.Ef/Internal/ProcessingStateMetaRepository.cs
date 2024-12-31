using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Internal;

class ProcessingStateMetaRepository : IProcessingStateMetaRepository
{
    private MetaDbContext DbContext { get; }
    
    public ProcessingStateMetaRepository(MetaDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListForEntityAsync(Guid tenantId, string entity)
    {
        return await Task.FromResult(DbContext.ProcessingStates
            .Where(p => p.TenantId == tenantId && p.Entity == entity)
            .OrderBy(c => c.State)
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired }));
    }
    
    public virtual async Task<ProcessingStateSelectListEntry?> SelectByStateAsync(Guid tenantId, string entity, int state)
    {
        return await Task.FromResult(DbContext.ProcessingStates
            .SingleOrDefault(c => c.TenantId == tenantId && c.Entity == entity && c.State == state)
            .As(c => c != null ? new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired } : null));
    }
    
    public virtual async Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListPossibleSuccessorsForEntityAsync(Guid tenantId, string entity, int state)
    {
        var currentState = await DbContext.ProcessingStates
            .SingleAsync(p => p.TenantId == tenantId && p.Entity == entity && p.State == state);
        
        var possibleSuccessors = !string.IsNullOrEmpty(currentState.Successors)
            ? JsonConvert.DeserializeObject<Guid[]>(currentState.Successors) ?? Array.Empty<Guid>() : Array.Empty<Guid>();

        return DbContext.ProcessingStates.Where(p => p.TenantId == tenantId && possibleSuccessors.Contains(p.Uuid))
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired });
    }
    
    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, State, Name from ProcessingState where TenantId='{tenantId}'");
    }
}