using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using AutoMapper;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class TenantMetaEndpoint
{
    private const string ApiTag = "Tenant";
    private const string ApiOperationPrefix = "Tenant";
    
    public static IEndpointRouteBuilder MapTenantMetaApi(this IEndpointRouteBuilder app, 
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta")
    {
        app.MapGet(basePath + "/metadatafortenant/{tenantId}", HandleMetadataForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<MetaTenant>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Metadata")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query tenant metadata by id");
        
        app.MapGet(basePath + "/allowed", HandleAllowedTenantsForUserAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<TenantSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "Allowed")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query allowed tenants for user");

        app.MapGet(basePath + "/selectlist", HandleSelectListAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<TenantSelectListEntry>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectList")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query list of all tenants");
        
        app.MapGet(basePath + "/selectbyid/{id}", HandleSelectByIdAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<TenantSelectListEntry>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "SelectById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query select item by id");
        
        return app;
    }

    public static IEndpointRouteBuilder MapTenantServiceApi(this IEndpointRouteBuilder app,
        string basePath,
        string apiTag = ApiTag,
        string apiOperationPrefix = ApiOperationPrefix,
        string authorizationScope = "serviceApi",
        string apiGroup = "service")
    {
        app.MapGet(basePath + "/servicemetadatafortenant/{tenantId}", HandleServiceMetadataForTenantAsync)
            .RequireAuthorization(authorizationScope)
            .Produces<ServiceTenant>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "ServiceMetadata")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query tenant metadata by id");
        
        app.MapGet(basePath + "/reportmetadatasourcesfortenant/{tenantId}", HandleReportMetaDatasourcesForTenant)
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<ServiceTenantReportDatasourceDefinition>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName(apiOperationPrefix + "ReportMetaDatasources")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query report meta datasources for tenant");
        
        return app;
    }

    private static async Task<IResult> HandleMetadataForTenantAsync(IMapper mapper, IPrincipalUtils principalUtils, ClaimsPrincipal user,
        ITenantMetaRepository tenantMetaRepository, Guid tenantId)
    {
        var entry = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        var userTenantId = principalUtils.GetUserTenandId(user);

        if (userTenantId != tenantId)
        {
            return Results.Forbid();
        }

        return Results.Ok(mapper.Map<MetaTenant>(entry));
    }
    
    private static async Task<IResult> HandleSelectListAsync(ITenantMetaRepository repository)
    {
        return Results.Ok(await repository.SelectListAsync());
    }
    
    private static async Task<IResult> HandleSelectByIdAsync(ITenantMetaRepository repository, Guid id)
    {
        var entry = await repository.SelectByIdAsync(id);

        if (entry == null)
        {
            return Results.NotFound();
        }
            
        return Results.Ok(entry);
    }
    
    private static async Task<IResult> HandleServiceMetadataForTenantAsync(IMapper mapper, 
        ITenantMetaRepository tenantMetaRepository, Guid tenantId)
    {
        var entry = await tenantMetaRepository.ByIdAsync(tenantId);
        
        if (entry == null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(mapper.Map<ServiceTenant>(entry));
    }
    
    private static async Task<IResult> HandleAllowedTenantsForUserAsync(IPrincipalUtils principalUtils, ClaimsPrincipal user,
        ITenantMetaRepository tenantMetaRepository)
    {
        var claims = principalUtils.GetUserClaims(user);

        return Results.Ok(await tenantMetaRepository.AllowedTenantsAsync(claims));
    }
    
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "DI injection needed")]
    private static async Task<IResult> HandleReportMetaDatasourcesForTenant(IMapper mapper, IMetaDbConnectionFactory metaDbConnectionFactory,
        ITenantMetaRepository tenantMetaRepository, 
        IEntityMetaRepository entityMetaRepository, 
        ILookupMetaRepository lookupMetaRepository,
        IDocumentMetaRepository documentMetaRepository,
        IDocumentationMetaRepository documentationMetaRepository,
        IMlModelMetaRepository mlModelMetaRepository,
        INotificationMetaRepository notificationMetaRepository,
        IPageMetaRepository pageMetaRepository,
        IStatisticMetaRepository statisticMetaRepository,
        ISubscriptionMetaRepository subscriptionMetaRepository,
        IPickvalueMetaRepository pickvalueMetaRepository,
        IProcessingStateMetaRepository processingStateMetaRepository,
        Guid tenantId)
    {
        var metaConnectionString = metaDbConnectionFactory.ConnectionString;
        
        var schemaDefinitions = new List<ReportDatasourceDefinition>();

        var metaSchemaDefinition = new ReportDatasourceDefinition
        {
            Name = "Meta",
            ConnectionString = metaConnectionString,
            Tables = new[] {
                new ReportDatasourceTable { Name = "Pickvalue", Query = await pickvalueMetaRepository.GenerateListQueryAsync(tenantId) },
                new ReportDatasourceTable { Name = "ProcessingState", Query = await processingStateMetaRepository.GenerateListQueryAsync(tenantId) }
            }
        };

        schemaDefinitions.Add(metaSchemaDefinition);
        
        var metaLookupsSchemaDefinition = new ReportDatasourceDefinition
        {
            Name = "MetaLookups",
            ConnectionString = metaConnectionString,
            Tables = new []
                {
                    new ReportDatasourceTable { Name = "documentLookup", Query = await documentMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "documentationLookup", Query = await documentationMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "entityLookup", Query = await entityMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "lookupLookup", Query = await lookupMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "mlmodelLookup", Query = await mlModelMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "notificationLookup", Query = await notificationMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "pageLookup", Query = await pageMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "statisticLookup", Query = await statisticMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "subscriptionLookup", Query = await subscriptionMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "tenantLookup", Query = await tenantMetaRepository.GenerateListQueryAsync() },
                    
                    new ReportDatasourceTable { Name = "entityIdentifierLookup", Query = await entityMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "entityRightLookup", Query = await entityMetaRepository.GenerateRightsListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "entityStateLookup", Query = await processingStateMetaRepository.GenerateListQueryAsync(tenantId) },
                    new ReportDatasourceTable { Name = "entityPickvalueLookup", Query = await pickvalueMetaRepository.GenerateListQueryAsync(tenantId) },
                }
        };
        
        schemaDefinitions.Add(metaLookupsSchemaDefinition);
        
        return Results.Ok(mapper.Map<IEnumerable<ServiceTenantReportDatasourceDefinition>>(schemaDefinitions));
    }
}