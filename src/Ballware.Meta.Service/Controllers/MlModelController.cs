using System;
using System.Net;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MlModelController : ControllerBase
{
    private IMlModelMetaRepository ModelMetaRepository { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }

    public MlModelController(IMlModelMetaRepository modelMetaRepository, ITenantMetaRepository tenantMetaRepository)
    {
        ModelMetaRepository = modelMetaRepository;
        TenantMetaRepository = tenantMetaRepository;
    }

    [HttpGet]
    [Route("metadatabytenantandid/{tenant}/{id}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Query model metadata by tenant and id",
        Description = "",
        OperationId = "MetadataForMlModelByTenantAndId"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Model metadata", typeof(MlModel), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> ModelMetadataByTenantAndId(Guid tenant, Guid id)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        var model = await ModelMetaRepository.MetadataByTenantAndIdAsync(tenantMeta, id);

        return Ok(model);
    }

    [HttpGet]
    [Route("metadatabytenantandidentifier/{tenant}/{identifier}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Query model metadata by tenant and identifier",
        Description = "",
        OperationId = "MetadataForMlModelByTenantAndIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Model metadata", typeof(MlModel), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> ModelMetadataByTenantAndIdentifier(Guid tenant, string identifier)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        var model = await ModelMetaRepository.MetadataByTenantAndIdentifierAsync(tenantMeta, identifier);

        return Ok(model);
    }

    [HttpPost]
    [Route("savetrainingstatebehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Save training behalf of user",
        Description = "",
        OperationId = "SaveMlModelTrainingStateBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Save Training State")]
    public async Task<IActionResult> SaveTrainingStateBehalfOfUser(Guid tenant, Guid user, [FromBody] MlModelTrainingState trainingState)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        await ModelMetaRepository.SaveTrainingStateAsync(tenantMeta, user, trainingState);

        return Ok();
    }
}