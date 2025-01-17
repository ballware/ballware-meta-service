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
public class StatisticController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IStatisticMetaRepository MetaRepository { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }

    public StatisticController(IPrincipalUtils principalUtils, IStatisticMetaRepository metaRepository, ITenantMetaRepository tenantMetaRepository)
    {
        PrincipalUtils = principalUtils;
        MetaRepository = metaRepository;
        TenantMetaRepository = tenantMetaRepository;
    }

    [HttpGet]
    [Route("metadataforidentifier")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query metadata for statistic by identifier",
      Description = "",
      OperationId = "MetadataForStatisticByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Statistic metadata", typeof(Statistic), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> MetaDataForIdentifier([FromQuery] string identifier)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        try
        {
            return Ok(await MetaRepository.MetadataByIdentifierAsync(tenantId, identifier));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}