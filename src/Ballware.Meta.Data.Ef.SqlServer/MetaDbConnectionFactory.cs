using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Ballware.Meta.Data.Ef.SqlServer;

public class MetaDbConnectionFactory : IMetaDbConnectionFactory
{
    public string ConnectionString { get; }

    public MetaDbConnectionFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public IDbConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);

        connection.Open();

        return connection;
    }

    public async Task<IDbConnection> OpenConnectionAsync()
    {
        var connection = new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        return connection;
    }
}