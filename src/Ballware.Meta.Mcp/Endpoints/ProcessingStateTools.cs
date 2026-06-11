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

public static class ProcessingStateToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwareProcessingStateTools(this IToolRegistry registry)
    {
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_processingstate_list",
            Description = "Returns list of all processing states for current tenant",
            OutputSchema = JsonSchema.FromType<ProcessingStateList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = ProcessingStateTools.HandleProcessingStateListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "processingstate", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_processingstate_by_id",
            Description = "Returns processing state by given id for current tenant",
            OutputSchema = JsonSchema.FromType<ProcessingStateSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "id",
                    Description = "Unique id of the processing state",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = ProcessingStateTools.HandleProcessingStateByIdAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "processingstate", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_processingstate_listforentity",
            Description = "Returns all processing states for a given entity for current tenant",
            OutputSchema = JsonSchema.FromType<ProcessingStateList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "entity",
                    Description = "Entity identifier to retrieve processing states for",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = ProcessingStateTools.HandleProcessingStateListForEntityAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "processingstate", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_processingstate_bystateforentity",
            Description = "Returns a single processing state by numeric state code for a given entity for current tenant",
            OutputSchema = JsonSchema.FromType<ProcessingStateSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "entity",
                    Description = "Entity identifier to retrieve the processing state for",
                    Type = ToolParamType.String,
                    Required = true
                },
                new ToolParam()
                {
                    Name = "state",
                    Description = "Numeric state code",
                    Type = ToolParamType.Number,
                    Required = true
                }
            ],
            ExecuteAsync = ProcessingStateTools.HandleProcessingStateByStateForEntityAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "processingstate", "view")
        });

        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_processingstate_successorsforentityandstate",
            Description = "Returns all possible successor processing states for a given entity and current state for current tenant",
            OutputSchema = JsonSchema.FromType<ProcessingStateList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params =
            [
                new ToolParam()
                {
                    Name = "entity",
                    Description = "Entity identifier to retrieve successor states for",
                    Type = ToolParamType.String,
                    Required = true
                },
                new ToolParam()
                {
                    Name = "state",
                    Description = "Current numeric state code",
                    Type = ToolParamType.Number,
                    Required = true
                }
            ],
            ExecuteAsync = ProcessingStateTools.HandleProcessingStateSuccessorsForEntityAndStateAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("mcp", "processingstate", "view")
        });

        return registry;
    }
}

public class ProcessingStateTools
{
    public static async Task<ToolResult> HandleProcessingStateListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var processingStateRepository = serviceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var states = await processingStateRepository.SelectListForTenantAsync(tenantId);
        var result = mapper.Map<ProcessingStateSummary[]>(states);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new ProcessingStateList()
        {
            ProcessingStates = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleProcessingStateByIdAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
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
        var processingStateRepository = serviceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var state = await processingStateRepository.SelectByIdForTenantAsync(tenantId, id);

        if (state == null)
        {
            return ToolResult.FromText($"Processing state not found with id {id}");
        }

        var result = mapper.Map<ProcessingStateSummary>(state);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleProcessingStateListForEntityAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("entity", out var entityObj) || entityObj is not string entity || string.IsNullOrEmpty(entity))
        {
            return ToolResult.FromText("Missing or invalid 'entity' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var processingStateRepository = serviceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var states = await processingStateRepository.SelectListForEntityAsync(tenantId, entity);
        var result = mapper.Map<ProcessingStateSummary[]>(states);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new ProcessingStateList()
        {
            ProcessingStates = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleProcessingStateByStateForEntityAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("entity", out var entityObj) || entityObj is not string entity || string.IsNullOrEmpty(entity))
        {
            return ToolResult.FromText("Missing or invalid 'entity' argument");
        }

        if (!arguments.TryGetValue("state", out var stateObj) || !TryParseInt(stateObj, out var stateValue))
        {
            return ToolResult.FromText("Missing or invalid 'state' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var processingStateRepository = serviceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var state = await processingStateRepository.SelectByStateAsync(tenantId, entity, stateValue);

        if (state == null)
        {
            return ToolResult.FromText($"Processing state not found for entity '{entity}', state {stateValue}");
        }

        var result = mapper.Map<ProcessingStateSummary>(state);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }

    public static async Task<ToolResult> HandleProcessingStateSuccessorsForEntityAndStateAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null)
        {
            return ToolResult.FromText("User is not authenticated");
        }

        if (!arguments.TryGetValue("entity", out var entityObj) || entityObj is not string entity || string.IsNullOrEmpty(entity))
        {
            return ToolResult.FromText("Missing or invalid 'entity' argument");
        }

        if (!arguments.TryGetValue("state", out var stateObj) || !TryParseInt(stateObj, out var stateValue))
        {
            return ToolResult.FromText("Missing or invalid 'state' argument");
        }

        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var processingStateRepository = serviceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var states = await processingStateRepository.SelectListPossibleSuccessorsForEntityAsync(tenantId, entity, stateValue);
        var result = mapper.Map<ProcessingStateSummary[]>(states);

        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new ProcessingStateList()
        {
            ProcessingStates = result.ToList()
        }, new JsonSerializerOptions()
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
