using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Mcp.Public;
using Ballware.Shared.Authorization;
using Ballware.Shared.Mcp;
using Ballware.Shared.Mcp.Authorization;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;

namespace Ballware.Meta.Mcp.Endpoints;

public static class EntityToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwareEntityTools(this IToolRegistry registry)
    {
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_entity_list",
            Description = "Returns list of available entities for current tenant",
            OutputSchema = JsonSchema.FromType<EntityList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = EntityTools.HandleEntityListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "entity", "view")
        });
        
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_entity_by_identifier",
            Description = "Returns entity metadata by given identifier for entity",
            OutputSchema = JsonSchema.FromType<EntitySummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [
                new ToolParam()
                {
                    Name = "identifier",
                    Description = "Unique identifier (technical name) for entity used for referencing this entity",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = EntityTools.HandleEntityByIdentifierAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "entity", "view")
        });
        
        return registry;
    }
}

public class EntityTools
{   
    public static async Task<ToolResult> HandleEntityListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var entityRepository = serviceProvider.GetRequiredService<IEntityMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var tenantId = principalUtils.GetUserTenandId(principal);

        var entities = await entityRepository.SelectListForTenantAsync(tenantId);

        var result = mapper.Map<EntitySummary[]>(entities);
        
        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new EntityList()
        {
            Entities = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
    
    public static async Task<ToolResult> HandleEntityByIdentifierAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        if (!arguments.TryGetValue("identifier", out var identifierObj) || identifierObj is not string entityIdentifier)
        {
            return ToolResult.FromText("Missing or invalid 'identifier' argument");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var entityRepository = serviceProvider.GetRequiredService<IEntityMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var tenantId = principalUtils.GetUserTenandId(principal);

        var entity = await entityRepository.SelectByIdentifierForTenantAsync(tenantId, entityIdentifier);

        if (entity == null)
        {
            return ToolResult.FromText($"Entity not found with identifier {entityIdentifier}");
        }

        var result = mapper.Map<EntitySummary>(entity);
        
        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
}