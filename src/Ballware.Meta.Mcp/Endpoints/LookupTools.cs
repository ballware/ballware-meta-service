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

public static class LookupToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwareLookupTools(this IToolRegistry registry)
    {
        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.lookup.list",
            Description = "Returns list of available lookups for current tenant",
            OutputSchema = JsonSchema.FromType<LookupList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = LookupTools.HandleLookupListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "lookup", "view")
        });

        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.lookup.by.id",
            Description = "Returns lookup summary by given id for lookup",
            OutputSchema = JsonSchema.FromType<LookupSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "id",
                    Description = "Unique id for lookup",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = LookupTools.HandleLookupByIdAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "lookup", "view")
        });
        
        return registry;
    }
}


public class LookupTools
{
    public static async Task<ToolResult> HandleLookupListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var lookupRepository = serviceProvider.GetRequiredService<ILookupMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var lookups = await lookupRepository.SelectListForTenantAsync(tenantId);
        var result = mapper.Map<LookupSummary[]>(lookups);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new LookupList()
        {
            Lookups = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleLookupByIdAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("id", out var idObj) || idObj is not string idArg || !Guid.TryParse(idArg, out var id))
        {
            return ToolResult.FromText("Missing or invalid 'id' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var lookupRepository = serviceProvider.GetRequiredService<ILookupMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var lookup = await lookupRepository.SelectByIdForTenantAsync(tenantId, id);

        if (lookup == null)
        {
            return ToolResult.FromText($"Lookup not found with id {id}");
        }

        var result = mapper.Map<LookupSummary>(lookup);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
}
