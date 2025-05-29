using System.Collections.Immutable;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Http;

namespace Ballware.Meta.Api.Public;

public class EditingEndpointBuilder
{
    private string Application { get; }
    private string Entity { get; }
    private Guid? TenantId { get; }
    private IDictionary<string, object> Claims { get; set; } = ImmutableDictionary<string, object>.Empty;
    private ITenantRightsChecker TenantRightsChecker { get; }
    private IEntityRightsChecker EntityRightsChecker { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private IEntityMetaRepository EntityMetaRepository { get; }
    private string? Right { get; set; }
    private bool CheckTenantRight { get; set; }
    private bool CheckEntityRight { get; set; }
    
    private object? EntityRightParam { get; set; }
    
    private EditingEndpointBuilder(ITenantMetaRepository tenantMetaRepository, IEntityMetaRepository entityMetaRepository, ITenantRightsChecker tenantRightsChecker, IEntityRightsChecker entityRightsChecker, Guid tenantId, string application, string entity)
    {
        TenantMetaRepository = tenantMetaRepository;
        EntityMetaRepository = entityMetaRepository;
        TenantRightsChecker = tenantRightsChecker;
        EntityRightsChecker = entityRightsChecker;
        TenantId = tenantId;
        Application = application;
        Entity = entity;
    }

    public static EditingEndpointBuilder Create(ITenantMetaRepository tenantMetaRepository, IEntityMetaRepository entityMetaRepository, ITenantRightsChecker tenantRightsChecker, IEntityRightsChecker entityRightsChecker, Guid tenantId, string application, string entity)
    {
        return new EditingEndpointBuilder(tenantMetaRepository, entityMetaRepository, tenantRightsChecker, entityRightsChecker, tenantId, application, entity);
    }
    
    public EditingEndpointBuilder WithClaims(IDictionary<string, object> claims)
    {
        Claims = claims;
        return this;
    }
    
    public EditingEndpointBuilder WithTenantAndEntityRightCheck(string right, object param)
    {
        Right = right;
        EntityRightParam = param;
        CheckTenantRight = true;
        CheckEntityRight = true;
        
        return this;
    }
    
    public async Task<IResult> ExecuteAsync(Func<Task<IResult>> executor)
    {
        if (!string.IsNullOrEmpty(Right) && TenantId != null && CheckTenantRight)
        {
            var tenant = await TenantMetaRepository.ByIdAsync(TenantId.Value);

            if (tenant == null)
            {
                return Results.NotFound($"Tenant {TenantId} not found");
            }
        
            var authorized = await TenantRightsChecker.HasRightAsync(tenant, Application, Entity, Claims, Right);

            if (CheckEntityRight)
            {
                var entity = await EntityMetaRepository.ByEntityAsync(TenantId.Value, Entity);
                
                if (entity == null)
                {
                    return Results.NotFound($"Entity {Entity} not found for tenant {TenantId}");
                }
                
                authorized = await EntityRightsChecker.HasRightAsync(TenantId.Value, entity, Claims, Right, EntityRightParam, authorized);
            }
            
            if (!authorized)
            {
                return Results.Unauthorized();
            }
        }
        
        return await executor();
    }
}