namespace Ballware.Meta.Data.Repository;

public interface ITenantableRepositoryHook<TEditable, TPersistable>
{
    void BeforeSave(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value,
        bool insert);

    void AfterSave(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value,
        TPersistable persistable, bool insert);

    RemoveResult RemovePreliminaryCheck(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams);

    void BeforeRemove(Guid tenantId, Guid? userId, IDictionary<string, object> claims,
        TPersistable persistable);
}