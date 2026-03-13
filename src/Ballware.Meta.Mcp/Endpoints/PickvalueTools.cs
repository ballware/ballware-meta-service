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

public static class PickvalueToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwarePickvalueTools(this IToolRegistry registry)
    {
        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.pickvalue.availability",
            Description = "Returns list of available entity/field combinations with pickvalues for current tenant",
            OutputSchema = JsonSchema.FromType<PickvalueAvailabilityList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = PickvalueTools.HandlePickvalueAvailabilityAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "pickvalue", "view")
        });

        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.pickvalue.selectlistforentityandfield",
            Description = "Returns list of pickvalue entries for a given entity and field for current tenant",
            OutputSchema = JsonSchema.FromType<PickvalueSelectList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "entity",
                    Description = "Entity name to retrieve pickvalues for",
                    Type = ToolParamType.String,
                    Required = true
                },
                new ToolParam()
                {
                    Name = "field",
                    Description = "Field name to retrieve pickvalues for",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = PickvalueTools.HandlePickvalueSelectListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "pickvalue", "view")
        });

        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.pickvalue.selectbyvalueforentityandfield",
            Description = "Returns a single pickvalue entry by numeric value for a given entity and field for current tenant",
            OutputSchema = JsonSchema.FromType<PickvalueSelectEntrySummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "entity",
                    Description = "Entity name to retrieve pickvalue for",
                    Type = ToolParamType.String,
                    Required = true
                },
                new ToolParam()
                {
                    Name = "field",
                    Description = "Field name to retrieve pickvalue for",
                    Type = ToolParamType.String,
                    Required = true
                },
                new ToolParam()
                {
                    Name = "value",
                    Description = "Numeric value of the pickvalue entry",
                    Type = ToolParamType.Number,
                    Required = true
                }
            ],
            ExecuteAsync = PickvalueTools.HandlePickvalueSelectByValueAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "pickvalue", "view")
        });

        return registry;
    }
}

public class PickvalueTools
{
    public static async Task<ToolResult> HandlePickvalueAvailabilityAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var pickvalueRepository = serviceProvider.GetRequiredService<IPickvalueMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var availabilities = await pickvalueRepository.GetPickvalueAvailabilityAsync(tenantId);
        var result = mapper.Map<PickvalueAvailabilitySummary[]>(availabilities);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new PickvalueAvailabilityList()
        {
            Availabilities = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandlePickvalueSelectListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("entity", out var entityObj) || entityObj is not string entity || string.IsNullOrEmpty(entity))
        {
            return ToolResult.FromText("Missing or invalid 'entity' argument");
        }

        if (!arguments.TryGetValue("field", out var fieldObj) || fieldObj is not string field || string.IsNullOrEmpty(field))
        {
            return ToolResult.FromText("Missing or invalid 'field' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var pickvalueRepository = serviceProvider.GetRequiredService<IPickvalueMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var pickvalues = await pickvalueRepository.SelectListForEntityFieldAsync(tenantId, entity, field);
        var result = mapper.Map<PickvalueSelectEntrySummary[]>(pickvalues);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new PickvalueSelectList()
        {
            Pickvalues = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandlePickvalueSelectByValueAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("entity", out var entityObj) || entityObj is not string entity || string.IsNullOrEmpty(entity))
        {
            return ToolResult.FromText("Missing or invalid 'entity' argument");
        }

        if (!arguments.TryGetValue("field", out var fieldObj) || fieldObj is not string field || string.IsNullOrEmpty(field))
        {
            return ToolResult.FromText("Missing or invalid 'field' argument");
        }

        if (!arguments.TryGetValue("value", out var valueObj) || !TryParseInt(valueObj, out var value))
        {
            return ToolResult.FromText("Missing or invalid 'value' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var pickvalueRepository = serviceProvider.GetRequiredService<IPickvalueMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var pickvalue = await pickvalueRepository.SelectByValueAsync(tenantId, entity, field, value);

        if (pickvalue == null)
        {
            return ToolResult.FromText($"Pickvalue not found for entity '{entity}', field '{field}', value {value}");
        }

        var result = mapper.Map<PickvalueSelectEntrySummary>(pickvalue);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    private static bool TryParseInt(object? obj, out int value)
    {
        value = 0;
        return obj switch
        {
            int i => (value = i) == i,
            long l => (value = (int)l) == l,
            string s => int.TryParse(s, out value),
            _ => false
        };
    }
}
