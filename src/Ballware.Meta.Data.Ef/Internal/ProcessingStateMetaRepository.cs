using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Internal;

class ProcessingStateMetaRepository : TenantableBaseRepository<Public.ProcessingState, Persistables.ProcessingState>, IProcessingStateMetaRepository
{
    public ProcessingStateMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.ProcessingState, Persistables.ProcessingState>? hook = null) 
        : base(mapper, dbContext, hook) { }

    protected override IQueryable<ProcessingState> ListQuery(IQueryable<ProcessingState> query, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if ("entity".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            return query.Where(er => er.Entity == entity.ToString());
        }
        
        return base.ListQuery(query, identifier, claims, queryParams);
    }

    public virtual async Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListForTenantAsync(Guid tenantId)
    {
        return await Task.FromResult(Context.ProcessingStates.Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Entity).ThenBy(r => r.State)
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired }));
    }
    
    public virtual async Task<ProcessingStateSelectListEntry?> SelectByIdForTenantAsync(Guid tenantId, Guid id)
    {
        return await Context.ProcessingStates.Where(r => r.TenantId == tenantId && r.Uuid == id)
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired })
            .FirstOrDefaultAsync();
    }
    
    public virtual async Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListForEntityAsync(Guid tenantId, string entity)
    {
        return await Task.Run(() => Context.ProcessingStates
            .Where(p => p.TenantId == tenantId && p.Entity == entity)
            .OrderBy(c => c.State)
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired }));
    }

    public virtual async Task<ProcessingStateSelectListEntry?> SelectByStateAsync(Guid tenantId, string entity, int state)
    {
        return await Task.Run(() => Context.ProcessingStates
            .SingleOrDefault(c => c.TenantId == tenantId && c.Entity == entity && c.State == state)
            .As(c => c != null ? new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired } : null));
    }

    public virtual async Task<IEnumerable<ProcessingStateSelectListEntry>> SelectListPossibleSuccessorsForEntityAsync(Guid tenantId, string entity, int state)
    {
        var currentState = await Context.ProcessingStates
            .SingleAsync(p => p.TenantId == tenantId && p.Entity == entity && p.State == state);

        var possibleSuccessors = !string.IsNullOrEmpty(currentState.Successors)
            ? JsonConvert.DeserializeObject<Guid[]>(currentState.Successors) ?? Array.Empty<Guid>() : Array.Empty<Guid>();

        return Context.ProcessingStates.Where(p => p.TenantId == tenantId && possibleSuccessors.Contains(p.Uuid))
            .Select(c => new ProcessingStateSelectListEntry { Id = c.Uuid, State = c.State, Name = c.Name, Locked = c.RecordLocked, Finished = c.RecordFinished, ReasonRequired = c.ReasonRequired });
    }

    public Task<string> GenerateListQueryAsync(Guid tenantId)
    {
        return Task.FromResult($"select Entity, State, Name from ProcessingState where TenantId='{tenantId}'");
    }
}