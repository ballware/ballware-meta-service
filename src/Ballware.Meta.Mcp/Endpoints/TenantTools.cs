using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Mcp.Public;
using Ballware.Shared.Authorization;
using Ballware.Shared.Mcp;
using Ballware.Shared.Mcp.Authorization;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;

namespace Ballware.Meta.Mcp.Endpoints;

public static class TenantToolRegistryExtensions
{
    public static IToolRegistry RegisterBallwareTenantTools(this IToolRegistry registry)
    {
        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.tenant.current.summary",
            Description = "Returns summarized metadata for current tenant of authenticated user",
            OutputSchema = JsonSchema.FromType<TenantSummary>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = TenantTools.HandleTenantCurrentSummaryAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "tenant", "view")
        });
        
        registry.RegisterTool(new Tool()
        {
            Name = "ballware.meta.tenant.current.navigation",
            Description = "Returns user navigation for current tenant of authenticated user",
            OutputSchema = JsonSchema.FromType<NavigationLayout>(JsonSchemaDefaults.SchemaSettings).ToJson(),
            Params = [],
            ExecuteAsync = TenantTools.HandleTenantCurrentNavigationAsync,
            IsAuthorizedAsync = McpAuthorizationHandlerFactory.CreateStaticEntityRightAuthorizationHandler("meta", "tenant", "view")
        });

        return registry;
    }
}

public class TenantTools
{ 
    public static async Task<ToolResult> HandleTenantCurrentSummaryAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var tenantRepository = serviceProvider.GetRequiredService<ITenantMetaRepository>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);

        var tenantData = await tenantRepository.ByIdAsync(tenantId);

        if (tenantData == null)
        {
            return ToolResult.FromText($"No tenant found for id {tenantId}");
        }

        var result = mapper.Map<TenantSummary>(tenantData);
        
        var structuredContent = JsonSerializer.SerializeToNode(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        if (structuredContent == null)
        {
            return ToolResult.FromText($"Error serializing summary for {tenantId}");
        }
        
        return ToolResult.FromStructuredContent(structuredContent);
    }
    
    public static async Task<ToolResult> HandleTenantCurrentNavigationAsync(IServiceProvider serviceProvider, ClaimsPrincipal? principal, IDictionary<string, object?> arguments)
    {
        if (principal == null) 
        {
            return ToolResult.FromText("User is not authenticated");
        }
        
        var principalUtils = serviceProvider.GetRequiredService<IPrincipalUtils>();
        var tenantRepository = serviceProvider.GetRequiredService<ITenantMetaRepository>();
        var tenantRightsChecker = serviceProvider.GetRequiredService<ITenantRightsChecker>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var tenantId = principalUtils.GetUserTenandId(principal);
        var claims = principalUtils.GetUserClaims(principal);

        var tenantData = await tenantRepository.ByIdAsync(tenantId);

        if (tenantData == null)
        {
            return ToolResult.FromText($"No tenant found for id {tenantId}");
        }

        var result = mapper.Map<TenantNavigation>(tenantData);

        result.Layout.Items = await FilterNavigationItemsAsync(result.Layout.Items, tenantData, tenantRightsChecker, claims);
        
        var structuredContent = JsonSerializer.SerializeToNode(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        if (structuredContent == null)
        {
            return ToolResult.FromText($"Error serializing navigation for {tenantId}");
        }
        
        return ToolResult.FromStructuredContent(structuredContent);
    }

    private static async Task<List<NavigationLayoutItem>> FilterNavigationItemsAsync(
        List<NavigationLayoutItem> items,
        Tenant tenant,
        ITenantRightsChecker tenantRightsChecker,
        IDictionary<string, object> claims)
    {
        var filteredItems = new List<NavigationLayoutItem>();

        foreach (var item in items)
        {
            switch (item.Type)
            {
                case NavigationLayoutItemType.Page:
                {
                    var pageIdentifier = item.Options?.Page;

                    if (!string.IsNullOrEmpty(pageIdentifier))
                    {
                        var hasRight = await tenantRightsChecker.HasRightAsync(tenant, "generic", "page", claims, pageIdentifier);

                        if (hasRight)
                        {
                            filteredItems.Add(item);
                        }
                    }

                    break;
                }
                case NavigationLayoutItemType.Group:
                case NavigationLayoutItemType.Section:
                {
                    if (item.Items is { Count: > 0 })
                    {
                        item.Items = await FilterNavigationItemsAsync(item.Items, tenant, tenantRightsChecker, claims);

                        if (item.Items.Count > 0)
                        {
                            filteredItems.Add(item);
                        }
                    }

                    break;
                }
            }
        }

        return filteredItems;
    }
    
}