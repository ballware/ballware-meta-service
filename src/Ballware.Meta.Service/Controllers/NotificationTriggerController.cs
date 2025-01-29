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
public class NotificationTriggerController : ControllerBase
{
    public IMapper Mapper { get; }
    public INotificationTriggerMetaRepository MetaRepository { get; }
    public ITenantMetaRepository TenantMetaRepository { get; }
    
    public NotificationTriggerController(IMapper mapper, INotificationTriggerMetaRepository notificationTriggerMetaRepository, ITenantMetaRepository tenantMetaRepository)
    {
        Mapper = mapper;
        MetaRepository = notificationTriggerMetaRepository;
        TenantMetaRepository = tenantMetaRepository;
    }
    
    [HttpGet]
    [Route("createnotificationtriggerfortenantandnotificationbehalfofuser/{tenant}/{notification}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Create new notification trigger for tenant behalf of user",
        Description = "",
        OperationId = "CreateNotificationTriggerForTenantAndNotificationBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Notification trigger data", typeof(NotificationTriggerDto), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> CreateForTenantBehalfOfUser(Guid tenant, Guid notification, Guid user)
    {
        var notificationTrigger = await MetaRepository.NewAsync(tenant, "primary", new Dictionary<string, object>());
        
        notificationTrigger.NotificationId = notification;

        return Ok(Mapper.Map<NotificationTriggerDto>(notificationTrigger));
    }
    
    [HttpPost]
    [Route("savenotificationtriggerbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
        Summary = "Save notification trigger behalf of user",
        Description = "",
        OperationId = "SaveNotificationTriggerBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Save notification trigger")]
    public async Task<IActionResult> SaveBehalfOfUser(Guid tenant, Guid user, [FromBody] NotificationTriggerDto notificationTrigger)
    {
        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        await MetaRepository.SaveAsync(tenant, user, "primary", new Dictionary<string, object>(), Mapper.Map<NotificationTrigger>(notificationTrigger));

        return Ok();
    }
}