using System;
using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DocumentationController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IDocumentationMetaRepository MetaRepository { get; }

    public DocumentationController(IPrincipalUtils principalUtils, IDocumentationMetaRepository metaRepository)
    {
        PrincipalUtils = principalUtils;
        MetaRepository = metaRepository;
    }

    [HttpGet]
    [Route("documentationforentity/{entity}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query documentation for entity",
      Description = "",
      OperationId = "DocumentationForEntity"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Documentation content", typeof(string), new[] { MimeMapping.KnownMimeTypes.Html })]
    public async Task<IActionResult> DocumentationForEntity(
      [SwaggerParameter("Entity identifier")] string entity)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        var content = (await MetaRepository.ByEntityAndFieldAsync(tenantId, entity, string.Empty))?.Content;

        if (content == null)
        {
            return NoContent();
        }
        
        return Content(content);
    }

    [HttpGet]
    [Route("documentationforentityandfield/{entity}/{field}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query documentation for entity and field",
      Description = "",
      OperationId = "DocumentationForEntityAndField"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Documentation content", typeof(string), new[] { MimeMapping.KnownMimeTypes.Html })]
    public async Task<IActionResult> DocumentationForEntityAndField(
      [SwaggerParameter("Entity identifier")] string entity,
      [SwaggerParameter("Field identifier")] string field
    )
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        var content = (await MetaRepository.ByEntityAndFieldAsync(tenantId, entity, field))?.Content;

        if (content == null)
        {
            return NoContent();
        }

        return Content(content);
    }
}