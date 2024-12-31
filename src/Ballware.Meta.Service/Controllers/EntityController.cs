using System;
using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EntityController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IEntityMetaRepository Repository { get; }

    public EntityController(IPrincipalUtils principalUtils, IEntityMetaRepository repository)
    {
        PrincipalUtils = principalUtils;
        Repository = repository;
    }

    [HttpGet]
    [Route("metadataforentity/{entity}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query metadata for entity",
      Description = "",
      OperationId = "MetadataForEntityByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Entity metadata for client operations", typeof(EntityMetadata), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> MetadataForEntity(
      [SwaggerParameter("Entity metadata identifier")] string entity
    )
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        var result = await Repository.ByEntityAsync(tenantId, entity);

        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpGet]
    [Route("selectlistrights")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query list of all defined entity rights",
      Description = "",
      OperationId = "EntityRights"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of all defined entity rights", typeof(IEnumerable<EntityRightSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListClaims()
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await Repository.SelectListEntityRightsAsync(tenantId));
    }
}