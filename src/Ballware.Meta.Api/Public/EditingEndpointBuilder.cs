using System.Collections.Immutable;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Http;

namespace Ballware.Meta.Api.Public;

public class EditingEndpointBuilder
{
    private string Application { get; }
    private string Entity { get; }
    private Guid? TenantId { get; set; }
    private IDictionary<string, object> Claims { get; set; } = ImmutableDictionary<string, object>.Empty;
    private ITenantRightsChecker TenantRightsChecker { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private string? Right { get; set; }
    
    private EditingEndpointBuilder(ITenantMetaRepository tenantMetaRepository, ITenantRightsChecker tenantRightsChecker, string application, string entity)
    {
        TenantMetaRepository = tenantMetaRepository;
        TenantRightsChecker = tenantRightsChecker;
        Application = application;
        Entity = entity;
    }

    public static EditingEndpointBuilder Create(ITenantMetaRepository tenantMetaRepository, ITenantRightsChecker tenantRightsChecker, string application, string entity)
    {
        return new EditingEndpointBuilder(tenantMetaRepository, tenantRightsChecker, application, entity);
    }
    
    public EditingEndpointBuilder WithClaims(IDictionary<string, object> claims)
    {
        Claims = claims;
        return this;
    }

    public EditingEndpointBuilder CheckTenantRight(Guid tenantId, string right)
    {
        TenantId = tenantId;
        Right = right;
        
        return this;
    }
    
    public async Task<IResult> ExecuteAsync(Func<Task<IResult>> executor)
    {
        if (!string.IsNullOrEmpty(Right) && TenantId != null)
        {
            var tenant = await TenantMetaRepository.ByIdAsync(TenantId.Value);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {TenantId} not found");
            }
            
            var tenantAuthorized = await TenantRightsChecker.HasRightAsync(tenant, Application, Entity, Claims, Right);
        
            if (!tenantAuthorized)
            {
                return Results.Unauthorized();
            }
        }
        
        return await executor();
    }
}