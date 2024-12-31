using System.Data;

namespace Ballware.Meta.Data;

public interface IMetaDbConnectionFactory
{
    string ConnectionString { get; }
    
    IDbConnection OpenConnection();
    Task<IDbConnection> OpenConnectionAsync();
}