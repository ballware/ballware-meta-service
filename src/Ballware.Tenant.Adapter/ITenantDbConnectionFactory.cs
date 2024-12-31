using System;
using System.Data;
using System.Threading.Tasks;

namespace Ballware.Tenant.Adapter;

public interface ITenantDbConnectionFactory
{
    IDbConnection OpenConnection(Meta.Data.Tenant tenant);
    Task<IDbConnection> OpenConnectionAsync(Guid tenantId);
    
    void RegisterConnection(Guid tenantId, string connectionString);
    void UnregisterConnection(Guid tenantId);
}