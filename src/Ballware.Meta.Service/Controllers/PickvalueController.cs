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
public class PickvalueController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IPickvalueMetaRepository Repository { get; }

    public PickvalueController(IPrincipalUtils principalUtils, IPickvalueMetaRepository repository)
    {
        PrincipalUtils = principalUtils;
        Repository = repository;
    }

    [HttpGet]
    [Route("selectlistforentityandfield/{entity}/{field}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query list of pickvalues for entity and field",
      Description = "",
      OperationId = "PickvaluesForEntityAndField"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of pickvalues", typeof(IEnumerable<PickvalueSelectEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> SelectListForEntityAndField(string entity, string field)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await Repository.SelectListForEntityFieldAsync(tenantId, entity, field));
    }

    [HttpGet]
    [Route("selectbyvalueforentityandfield/{entity}/{field}/{value}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query single pickvalue for entity and field by value",
      Description = "",
      OperationId = "PickvalueForEntityAndFieldByValue"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Single pickvalue", typeof(PickvalueSelectEntry), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> SelectByValueForEntityAndField(string entity, string field, int value)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        return Ok(await Repository.SelectByValueAsync(tenantId, entity, field, value));
    }

    [HttpGet]
    [Route("selectbyvaluefortenantandentityandfield/{tenant}/{entity}/{field}/{value}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query single pickvalue for tenant and entity and field by value",
      Description = "",
      OperationId = "PickvalueForTenantAndEntityAndFieldByValue"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Single pickvalue", typeof(PickvalueSelectEntry), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> SelectByValueForTenantAndEntityAndField(Guid tenant, string entity, string field, int value)
    {
        return Ok(await Repository.SelectByValueAsync(tenant, entity, field, value));
    }
}