using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

public class JobCreatePayload
{
    public required string Scheduler { get; set; }
    public required string Identifier { get; set; }
    public required string Options { get; set; }
}

public class JobUpdatePayload
{
    public Guid Id { get; set; }
    public JobStates State { get; set; }
    public required string Result { get; set; }
}

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IJobMetaRepository MetaRepository { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    
    public JobController(IPrincipalUtils principalUtils, IJobMetaRepository metaRepository, ITenantMetaRepository tenantMetaRepository)
    {
        PrincipalUtils = principalUtils;
        MetaRepository = metaRepository;
        TenantMetaRepository = tenantMetaRepository;
    }
    
    [HttpGet]
    [Route("pendingjobsforuser")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
        Summary = "Query pending jobs for current user",
        Description = "",
        OperationId = "PendingJobsForUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of pending jobs for current user", typeof(IEnumerable<Job>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> PendingJobsForUser()
    {
        var currentUserId = PrincipalUtils.GetUserId(User);
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenantId);
        
        if (tenantMeta == null)
        {
            return NotFound();
        }
        
        return Ok(await MetaRepository.PendingJobsForUser(tenantMeta, currentUserId));
    }
    
    [HttpPost]
    [Route("createjobfortenantbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Create new background job for tenant behalf of user",
        Description = "",
        OperationId = "CreateJobForTenantBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Job data", typeof(Job), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> CreateJobForTenantBehalfOfUser(Guid tenant, Guid user, [FromBody] JobCreatePayload data)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);
        
        if (tenantMeta == null)
        {
            return NotFound();
        }
        
        var job = await MetaRepository.CreateJobAsync(tenantMeta, user, data.Scheduler, data.Identifier, data.Options);

        return Ok(job);
    }
    
    [HttpPost]
    [Route("updatejobfortenantbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Update background job for tenant behalf of user",
        Description = "",
        OperationId = "UpdateJobForTenantBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Job data", typeof(Job), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> UpdateJobForTenantBehalfOfUser(Guid tenant, Guid user, [FromBody] JobUpdatePayload data)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);
        
        if (tenantMeta == null)
        {
            return NotFound();
        }
        
        var job = await MetaRepository.UpdateJobAsync(tenantMeta, user, data.Id, data.State, data.Result);

        return Ok(job);
    }
    
}