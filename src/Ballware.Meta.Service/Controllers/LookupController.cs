using System;
using System.Net;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LookupController : ControllerBase
{
    private ILookupMetaRepository LookupMetaRepository { get; }

    public LookupController(ILookupMetaRepository lookupMetaRepository)
    {
      LookupMetaRepository = lookupMetaRepository;
    }

    [HttpGet]
    [Route("lookupmetadatabytenantandidentifier/{tenant}/{identifier}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query lookup metadata by tenant and identifier",
      Description = "",
      OperationId = "MetadataForLookupByTenantAndIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lookup metadata", typeof(Lookup), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> LookupMetadataByTenantAndId(Guid tenant, string identifier)
    {
        try
        {
          var lookup = await LookupMetaRepository.ByIdentifierAsync(tenant, identifier);

          if (lookup == null)
          {
            return NotFound();
          }
          
          return Ok(lookup);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}