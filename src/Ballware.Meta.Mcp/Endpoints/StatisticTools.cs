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

public static class StatisticToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwareStatisticTools(this IToolRegistry registry)
    {
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_statistic_list",
            Description = "Returns list of available statistics for current tenant",
            OutputSchema = JsonSchema.FromType<StatisticList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = StatisticTools.HandleStatisticListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "statistic", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_statistic_by_id",
            Description = "Returns statistic summary by given id for current tenant",
            OutputSchema = JsonSchema.FromType<StatisticSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "id",
                    Description = "Unique id of the statistic",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = StatisticTools.HandleStatisticByIdAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "statistic", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_statistic_metadata_by_identifier",
            Description = "Returns full metadata for a statistic by its identifier for current tenant",
            OutputSchema = JsonSchema.FromType<StatisticMetadata>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "identifier",
                    Description = "Identifier of the statistic",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = StatisticTools.HandleStatisticMetadataByIdentifierAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "statistic", "view")
        });

        return registry;
    }
}

public class StatisticTools
{
    public static async Task<ToolResult> HandleStatisticListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var statisticRepository = serviceProvider.GetRequiredService<IStatisticMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var statistics = await statisticRepository.SelectListForTenantAsync(tenantId);
        var result = mapper.Map<StatisticSummary[]>(statistics);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new StatisticList()
        {
            Statistics = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleStatisticByIdAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
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
        var statisticRepository = serviceProvider.GetRequiredService<IStatisticMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var statistic = await statisticRepository.SelectByIdForTenantAsync(tenantId, id);

        if (statistic == null)
        {
            return ToolResult.FromText($"Statistic not found with id {id}");
        }

        var result = mapper.Map<StatisticSummary>(statistic);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleStatisticMetadataByIdentifierAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("identifier", out var identifierObj) || identifierObj is not string identifier || string.IsNullOrEmpty(identifier))
        {
            return ToolResult.FromText("Missing or invalid 'identifier' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var statisticRepository = serviceProvider.GetRequiredService<IStatisticMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var statistic = await statisticRepository.MetadataByIdentifierAsync(tenantId, identifier);

        if (statistic == null)
        {
            return ToolResult.FromText($"Statistic not found with identifier '{identifier}'");
        }

        var result = mapper.Map<StatisticMetadata>(statistic);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
}
