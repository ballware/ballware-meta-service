using Microsoft.Data.SqlClient;

namespace Ballware.Meta.Tenant.Data.SqlServer.Internal;

static class Utils
{
    public static string GetConnectionString(Meta.Data.Public.Tenant tenant)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = tenant.Server,
            InitialCatalog = tenant.Catalog,
            UserID = tenant.User,
            Password = tenant.Password,
            Encrypt = true,
            PersistSecurityInfo = false,
            IntegratedSecurity = false,
        };

        return connectionStringBuilder.ConnectionString;
    }
}