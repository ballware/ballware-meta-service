namespace Ballware.Meta.Data.Repository;

public interface IRepositoryHook<TEditable, TPersistable>
{
    void BeforeSave(Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value, bool insert);

    void AfterSave(Guid? userId, string identifier, IDictionary<string, object> claims, TEditable value,
        TPersistable persistable, bool insert);

    RemoveResult RemovePreliminaryCheck(Guid? userId, IDictionary<string, object> claims,
        IDictionary<string, object> removeParams);

    void BeforeRemove(Guid? userId, IDictionary<string, object> claims, TPersistable persistable);
}