using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Schema.Client;
using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Service.Extensions;

public class GenericSchemaEntityRepositoryHook 
    : ITenantableRepositoryHook<Ballware.Meta.Data.Public.EntityMetadata, Ballware.Meta.Data.Persistables.EntityMetadata>
{
    private ILogger<GenericSchemaEntityRepositoryHook> Logger { get; }
    
    private BallwareSchemaClient SchemaClient { get; }
    
    public GenericSchemaEntityRepositoryHook(ILogger<GenericSchemaEntityRepositoryHook> logger, BallwareSchemaClient schemaClient)
    {
        Logger = logger;
        SchemaClient = schemaClient;
    }

    public void AfterSave(Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Data.Public.EntityMetadata value,
        EntityMetadata persistable, bool insert)
    {
        if (("importjson".Equals(identifier, StringComparison.InvariantCultureIgnoreCase) 
             || "providermodel".Equals(identifier, StringComparison.InvariantCultureIgnoreCase)
             || "primary".Equals(identifier, StringComparison.InvariantCultureIgnoreCase)) 
            && value.GeneratedSchema 
            && !string.IsNullOrEmpty(value.ProviderModelDefinition))
        {
            SchemaClient.TenantCreateOrUpdateEntitySchemaForTenant(persistable.TenantId, new EntitySchema()
            {
                UserId = userId,
                SerializedEntityModel = value.ProviderModelDefinition
            });
        }
    }

    public void BeforeRemove(Guid tenantId, Guid? userId, IDictionary<string, object> claims, EntityMetadata persistable)
    {
        if (persistable.GeneratedSchema && !string.IsNullOrEmpty(persistable.Entity))
        {
            SchemaClient.TenantDropEntitySchemaForTenant(persistable.TenantId, persistable.Entity, userId);
        }
    }
}