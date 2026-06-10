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

public static class PageToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwarePageTools(this IToolRegistry registry)
    {
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_page_list",
            Description = "Returns list of available pages for current tenant",
            OutputSchema = JsonSchema.FromType<PageList>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = PageTools.HandlePageListAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "page", "view")
        });
        
        registry.RegisterStaticTool(new Tool()
        {
            Name = "ballware_meta_page_by_identifier",
            Description = "Returns page summary by given identifier for page",
            OutputSchema = JsonSchema.FromType<PageSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [
                new ToolParam()
                {
                    Name = "identifier",
                    Description = "Unique identifier for page used for referencing this page",
                    Type = ToolParamType.String,
                    Required = true
                }
            ],
            ExecuteAsync = PageTools.HandlePageByIdentifierAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "page", "view")
        });
        
        return registry;
    }
}

public class PageTools
{   
    public static async Task<ToolResult> HandlePageListAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var pageRepository = serviceProvider.GetRequiredService<IPageMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var tenantId = principalUtils.GetUserTenandId(principal);

        var pages = await pageRepository.SelectListForTenantAsync(tenantId);

        var result = mapper.Map<PageSummary[]>(pages);
        
        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(new PageList()
        {
            Pages = result.ToList()
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
    
    public static async Task<ToolResult> HandlePageByIdentifierAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        if (!arguments.TryGetValue("identifier", out var identifierObj) || identifierObj is not string pageIdentifier)
        {
            return ToolResult.FromText("Missing or invalid 'identifier' argument");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var pageRepository = serviceProvider.GetRequiredService<IPageMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var tenantId = principalUtils.GetUserTenandId(principal);

        var page = await pageRepository.ByIdentifierAsync(tenantId, pageIdentifier);

        if (page == null)
        {
            return ToolResult.FromText($"Page not found with identifier {pageIdentifier}");
        }

        var result = mapper.Map<PageSummary>(page);
        
        return ToolResult.FromStructuredContent(JsonSerializer.SerializeToElement(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }));
    }
}