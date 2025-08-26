using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace Ballware.Meta.Data.Ef.Postgres;

public class MetaDbConnectionFactory : IMetaDbConnectionFactory
{
    public string Provider { get; }
    public string ConnectionString { get; }

    public MetaDbConnectionFactory(string connectionString)
    {
        Provider = "postgres";
        ConnectionString = connectionString;
    }
}
