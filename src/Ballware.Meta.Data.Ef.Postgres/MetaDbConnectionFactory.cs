using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace Ballware.Meta.Data.Ef.Postgres;

public class MetaDbConnectionFactory : IMetaDbConnectionFactory
{
    public string ConnectionString { get; }

    public MetaDbConnectionFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public IDbConnection OpenConnection()
    {
        var connection = new NpgsqlConnection(ConnectionString);

        connection.Open();

        return connection;
    }

    public async Task<IDbConnection> OpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();

        return connection;
    }
}
