using System;
using System.Net;
using Ballware.Meta.Authorization;
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
public class ProcessingStateController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IProcessingStateMetaRepository MetaRepository { get; }

    public ProcessingStateController(IPrincipalUtils principalUtils, IProcessingStateMetaRepository metaRepository)
    {
        PrincipalUtils = principalUtils;
        MetaRepository = metaRepository;
    }

    [HttpGet]
    [Route("selectlistforentity/{entity}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query all processing states for entity ",
      Description = "",
      OperationId = "AllProcessingStatesForEntityByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of all defined processing states", typeof(IEnumerable<ProcessingStateSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListForEntity(string entity)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await MetaRepository.SelectListForEntityAsync(tenantId, entity));
    }

    [HttpGet]
    [Route("selectbystateforentity/{entity}/{state}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query single processing state for entity by state value",
      Description = "",
      OperationId = "SingleProcessingStateForEntityByValue"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Single processing state", typeof(ProcessingStateSelectListEntry), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectByStateForEntity(string entity, int state)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await MetaRepository.SelectByStateAsync(tenantId, entity, state));
    }

    [HttpGet]
    [Route("selectbystatefortenantandentity/{tenant}/{entity}/{state}")]
    [Authorize("serviceApi")]
    [ApiExplorerSettings(GroupName = "service")]
    [SwaggerOperation(
      Summary = "Query single processing state for entity by state value",
      Description = "",
      OperationId = "SingleProcessingStateForTenantAndEntityByValue"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Single processing state", typeof(ProcessingStateSelectListEntry), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectByStateForTenantAndEntity(Guid tenant, string entity, int state)
    {
        return Ok(await MetaRepository.SelectByStateAsync(tenant, entity, state));
    }

    [HttpGet]
    [Route("selectlistallsuccessorsforentityandstate/{entity}/{state}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query possible successing processing states for entity by state value",
      Description = "",
      OperationId = "SuccessingProcessingStatesForEntityByValue"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of successing processing states", typeof(IEnumerable<ProcessingStateSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListAllSuccessorsForEntityAndState(string entity, int state)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await MetaRepository.SelectListPossibleSuccessorsForEntityAsync(tenantId, entity, state));
    }
}