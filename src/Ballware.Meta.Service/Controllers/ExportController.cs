using System;
using System.Net;
using AutoMapper;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Service.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ExportController : ControllerBase
{
    public IMapper Mapper { get; }
    public IExportMetaRepository MetaRepository { get; }
    public ITenantMetaRepository TenantMetaRepository { get; }
    
    public ExportController(IMapper mapper, IExportMetaRepository exportMetaRepository, ITenantMetaRepository tenantMetaRepository)
    {
        Mapper = mapper;
        MetaRepository = exportMetaRepository;
        TenantMetaRepository = tenantMetaRepository;
    }
    
    [HttpGet]
    [Route("createexportfortenantbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Create new export for tenant behalf of user",
        Description = "",
        OperationId = "CreateExportForTenantBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Export data", typeof(ServiceExportDto), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> CreateForTenantBehalfOfUser(Guid tenant, Guid user)
    {
        var exportMeta = await MetaRepository.NewAsync(tenant, "primary", new Dictionary<string, object>());

        return Ok(exportMeta);
    }
    
    [HttpGet]
    [Route("exportbyidfortenant/{tenant}/{id}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Fetch export for tenant behalf of user",
        Description = "",
        OperationId = "FetchExportByIdForTenant"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Export not found")]
    [SwaggerResponse((int)HttpStatusCode.OK, "Export data", typeof(ServiceExportDto), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> FetchExportByIdForTenant(Guid tenant, Guid id)
    {
        var exportMeta = await MetaRepository.ByIdAsync(tenant, "primary", new Dictionary<string, object>(), id);

        if (exportMeta == null)
        {
            return NotFound("Export not found");
        }
        
        return Ok(Mapper.Map<ServiceExportDto>(exportMeta));
    }
    
    [HttpPost]
    [Route("saveexportbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Save export behalf of user",
        Description = "",
        OperationId = "SaveExportBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Save export")]
    public async Task<IActionResult> SaveBehalfOfUser(Guid tenant, Guid user, [FromBody] ServiceExportDto export)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        await MetaRepository.SaveAsync(tenant, user, "primary", new Dictionary<string, object>(), Mapper.Map<Export>(export));

        return Ok();
    }
}