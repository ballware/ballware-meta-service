using System.Text;
using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Internal;

class TenantableBaseRepository<TEditable, TPersistable> : ITenantableRepository<TEditable> where TEditable : class, IEditable, new() where TPersistable : class, IEntity, ITenantable, new()
{
    protected IMapper Mapper { get; }
    protected MetaDbContext Context { get; }

    protected TenantableBaseRepository(IMapper mapper, MetaDbContext dbContext)
    {
        Mapper = mapper;
        Context = dbContext;
    }

    protected virtual IQueryable<TPersistable> ListQuery(IQueryable<TPersistable> query, string identifier,
        IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if (queryParams.TryGetValue("id", out var param))
        {
            var ids = (param as IEnumerable<string>)?.Select(Guid.Parse) ?? new List<Guid>();

            query = query.Where(x => ids.Contains(x.Uuid));
        }

        return query;
    }

    protected virtual IQueryable<TPersistable> ByIdQuery(IQueryable<TPersistable> query, string identifier,
        IDictionary<string, object> claims, Guid id)
    {
        return query;
    }

    protected virtual TPersistable New(string identifier, IDictionary<string, object> claims, IDictionary<string, object>? queryParams)
    {
        return new TPersistable()
        {
            Uuid = Guid.NewGuid(),
        };
    }

    protected virtual TEditable ById(string identifier, IDictionary<string, object> claims, TEditable value)
    {
        return value;
    }

    protected virtual void BeforeSave(Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value, bool insert) { }
    protected virtual void AfterSave(Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value, TPersistable persistable, bool insert) { }
    protected virtual RemoveResult RemovePreliminaryCheck(Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams)
    {
        return new RemoveResult()
        {
            Result = true,
            Messages = Array.Empty<string>()
        };
    }

    protected virtual void BeforeRemove(Guid? userId, IDictionary<string, object> claims,
        TPersistable persistable)
    { }

    public Task<IEnumerable<TEditable>> AllAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return Task.Run(() => Context.Set<TPersistable>().Where(t => t.TenantId == tenantId).AsEnumerable().Select(Mapper.Map<TEditable>));
    }

    public Task<IEnumerable<TEditable>> QueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return Task.Run(() => ListQuery(Context.Set<TPersistable>().Where(t => t.TenantId == tenantId), identifier, claims, queryParams).AsEnumerable().Select(Mapper.Map<TEditable>));
    }

    public Task<long> CountAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return Task.Run(() =>
            ListQuery(Context.Set<TPersistable>().Where(t => t.TenantId == tenantId), identifier, claims, queryParams)
                .LongCount());
    }

    public Task<TEditable?> ByIdAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, Guid id)
    {
        return Task.Run(() =>
            ByIdQuery(Context.Set<TPersistable>().Where(t => t.TenantId == tenantId && t.Uuid == id), identifier,
                claims, id).AsEnumerable().Select(Mapper.Map<TEditable>).Select(e => ById(identifier, claims, e)).FirstOrDefault());
    }

    public Task<TEditable> NewAsync(Guid tenantId, string identifier, IDictionary<string, object> claims)
    {
        return Task.Run(() =>
        {
            var instance = New(identifier, claims, null);

            instance.TenantId = tenantId;

            return Mapper.Map<TEditable>(instance);
        });
    }

    public Task<TEditable> NewQueryAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        return Task.Run(() =>
        {
            var instance = New(identifier, claims, queryParams);

            instance.TenantId = tenantId;

            return Mapper.Map<TEditable>(instance);
        });
    }

    public async Task SaveAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value)
    {
        var persistedItem = await Context.Set<TPersistable>()
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Uuid == value.Id);

        var insert = persistedItem == null;

        BeforeSave(userId, identifier, claims, value, insert);

        if (persistedItem == null)
        {
            persistedItem = Mapper.Map<TPersistable>(value);
            persistedItem.TenantId = tenantId;

            if (persistedItem is IAuditable auditable)
            {
                auditable.CreatorId = userId;
                auditable.CreateStamp = DateTime.Now;
                auditable.LastChangerId = userId;
                auditable.LastChangeStamp = DateTime.Now;
            }

            Context.Set<TPersistable>().Add(persistedItem);
        }
        else
        {
            Mapper.Map(value, persistedItem);

            if (persistedItem is IAuditable auditable)
            {
                auditable.LastChangerId = userId;
                auditable.LastChangeStamp = DateTime.Now;
            }

            Context.Set<TPersistable>().Update(persistedItem);
        }

        AfterSave(userId, identifier, claims, value, persistedItem, insert);

        await Context.SaveChangesAsync();
    }

    public async Task<RemoveResult> RemoveAsync(Guid tenantId, Guid? userId, IDictionary<string, object> claims, IDictionary<string, object> removeParams)
    {
        var result = RemovePreliminaryCheck(userId, claims, removeParams);

        if (!result.Result)
        {
            return result;
        }

        if (removeParams.TryGetValue("Id", out var idParam) && Guid.TryParse(idParam.ToString(), out Guid id))
        {
            var persistedItem = await Context.Set<TPersistable>()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Uuid == id);

            if (persistedItem != null)
            {
                BeforeRemove(userId, claims, persistedItem);

                Context.Set<TPersistable>().Remove(persistedItem);

                await Context.SaveChangesAsync();
            }
        }

        return new RemoveResult() { Result = true, Messages = Array.Empty<string>() };
    }

    public async Task ImportAsync(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Stream importStream,
        Func<TEditable, Task<bool>> authorized)
    {
        using var textReader = new StreamReader(importStream);

        var items = JsonConvert.DeserializeObject<IEnumerable<TEditable>>(await textReader.ReadToEndAsync());

        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (await authorized(item))
            {
                await SaveAsync(tenantId, userId, identifier, claims, item);
            }
        }
    }

    public async Task<ExportResult> ExportAsync(Guid tenantId, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        var items = (await QueryAsync(tenantId, identifier, claims, queryParams)).Select(e => ById(identifier, claims, e));

        return new ExportResult()
        {
            FileName = $"{identifier}.json",
            Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items)),
            MediaType = "application/json",
        };
    }
}