using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Schema.Client;

namespace Ballware.Meta.Service.Extensions;

public class GenericSchemaTenantRepositoryHook 
    : IRepositoryHook<Ballware.Meta.Data.Public.Tenant, Ballware.Meta.Data.Persistables.Tenant>
{
    private ILogger<GenericSchemaTenantRepositoryHook> Logger { get; }
    
    private BallwareSchemaClient SchemaClient { get; }
    
    public GenericSchemaTenantRepositoryHook(ILogger<GenericSchemaTenantRepositoryHook> logger, BallwareSchemaClient schemaClient)
    {
        Logger = logger;
        SchemaClient = schemaClient;
    }
    
    public void AfterSave(Guid? userId, string identifier, IDictionary<string, object> claims, Ballware.Meta.Data.Public.Tenant value,
        Ballware.Meta.Data.Persistables.Tenant persistable, bool insert)
    {
        if (value.ManagedDatabase && !string.IsNullOrEmpty(value.ProviderModelDefinition))
        {
            SchemaClient.TenantCreateOrUpdateSchemaForTenant(value.Id, new TenantSchema()
            {
                Provider = value.Provider,
                UserId = userId,
                SerializedTenantModel = value.ProviderModelDefinition
            });    
        }
    }

    public void BeforeRemove(Guid? userId, IDictionary<string, object> claims, Tenant persistable)
    {
        if (persistable.ManagedDatabase)
        {
            SchemaClient.TenantDropSchemaForTenant(persistable.Uuid, userId);
        }
    }
}