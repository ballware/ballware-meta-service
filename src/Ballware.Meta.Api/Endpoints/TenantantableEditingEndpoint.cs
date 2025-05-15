using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ballware.Meta.Api.Endpoints;

public static class TenantableEditingEndpoint
{
    public static IEndpointRouteBuilder MapTenantableEditingApi<TEntity>(this IEndpointRouteBuilder app, 
        string basePath,
        string application,
        string entity,
        string apiTag,
        string apiOperationPrefix,
        string authorizationScope = "metaApi",
        string apiGroup = "meta" 
        ) where TEntity : class
    {
        app.MapGet(basePath + "/all", TenantableEndpointHandlerFactory.CreateAllHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<TEntity>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "All")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query all");
        
        app.MapGet(basePath + "/query", TenantableEndpointHandlerFactory.CreateQueryHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<IEnumerable<TEntity>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Query")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query items by query identifier and params");
        
        app.MapGet(basePath + "/new", TenantableEndpointHandlerFactory.CreateNewHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<TEntity>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "New")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query new tenant");
        
        app.MapGet(basePath + "/byid", TenantableEndpointHandlerFactory.CreateByIdHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<TEntity>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "ById")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query existing tenant");
        
        app.MapPost(basePath + "/save", TenantableEndpointHandlerFactory.CreateSaveHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Save")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Save existing or new tenant");
        
        app.MapDelete(basePath + "/remove/{id}", TenantableEndpointHandlerFactory.CreateRemoveHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<TEntity>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Remove")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Query existing tenant");
        
        app.MapPost(basePath + "/import", TenantableEndpointHandlerFactory.CreateImportHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Import")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Import from file");
        
        app.MapGet(basePath + "/export", TenantableEndpointHandlerFactory.CreateExportHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Export")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Export by query");
        
        app.MapPost(basePath + "/exporturl", TenantableEndpointHandlerFactory.CreateExportUrlHandler<TEntity>(application, entity))
            .RequireAuthorization(authorizationScope)
            .DisableAntiforgery()
            .Accepts<IFormCollection>("application/x-www-form-urlencoded")
            .Produces<string>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "ExportUrl")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Export to file by query");
        
        app.MapGet(basePath + "/download", TenantableEndpointHandlerFactory.CreateDownloadExportHandler())
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .WithName(apiOperationPrefix + "Download")
            .WithGroupName(apiGroup)
            .WithTags(apiTag)
            .WithSummary("Download exported");
        
        return app;
    }
}