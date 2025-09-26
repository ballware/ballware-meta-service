using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Ballware.Meta.Data.Ef.SqlServer;

public class MetaDbConnectionFactory : IMetaDbConnectionFactory
{
    public string Provider { get; }
    public string ConnectionString { get; }

    public MetaDbConnectionFactory(string connectionString)
    {
        Provider = "mssql";
        ConnectionString = connectionString;
    }
}