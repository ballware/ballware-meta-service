using System;
using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PageController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IPageMetaRepository MetaRepository { get; }

    public PageController(IPrincipalUtils principalUtils, IPageMetaRepository metaRepository)
    {
        PrincipalUtils = principalUtils;
        MetaRepository = metaRepository;
    }

    [HttpGet]
    [Route("pagedataforidentifier/{identifier}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query metadata for page by identifier",
      Description = "",
      OperationId = "MetadataForPageByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Metadata for page", typeof(Page), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> PageDataForIdentifier(string identifier)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        var page = await MetaRepository.ByIdentifierAsync(tenantId, identifier);

        if (page == null)
        {
            return NotFound();
        }
        
        return Ok(page);
    }
}