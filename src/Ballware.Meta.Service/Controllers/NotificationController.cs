using System;
using System.Net;
using AutoMapper;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Service.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("metaApi", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationController : ControllerBase
    {
        private IMapper Mapper { get; }
        private INotificationMetaRepository NotificationMetaRepository { get; }

        public NotificationController(IMapper mapper,
            INotificationMetaRepository notificationMetaRepository)
        {
            Mapper = mapper;
            NotificationMetaRepository = notificationMetaRepository;
        }

        [HttpGet]
        [Route("notificationmetadatabytenantandid/{tenant}/{id}")]
        [Authorize("serviceApi")]
        [ApiExplorerSettings(GroupName = "service")]
        [SwaggerOperation(
          Summary = "Query notification metadata by tenant and id",
          Description = "",
          OperationId = "MetadataForNotificationByTenantAndId"
        )]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Notification metadata", typeof(ServiceNotificationDto), new[] { MimeMapping.KnownMimeTypes.Json })]
        public async Task<IActionResult> NotificationMetadataByTenantAndId(Guid tenant, Guid id)
        {
            var notification = await NotificationMetaRepository.MetadataByTenantAndIdAsync(tenant, id);

            if (notification == null)
            {
                return NotFound();
            }
            
            return Ok(Mapper.Map<ServiceNotificationDto>(notification));
        }
        
        [HttpGet]
        [Route("notificationmetadatabytenantandidentifier/{tenant}/{identifier}")]
        [Authorize("serviceApi")]
        [ApiExplorerSettings(GroupName = "service")]
        [SwaggerOperation(
            Summary = "Query notification metadata by tenant and identifier",
            Description = "",
            OperationId = "MetadataForNotificationByTenantAndIdentifier"
        )]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Notification metadata", typeof(ServiceNotificationDto), new[] { MimeMapping.KnownMimeTypes.Json })]
        public async Task<IActionResult> NotificationMetadataByTenantAndIdentifier(Guid tenant, string identifier)
        {
            var notification = await NotificationMetaRepository.MetadataByTenantAndIdentifierAsync(tenant, identifier);

            if (notification == null)
            {
                return NotFound();
            }
            
            return Ok(Mapper.Map<ServiceNotificationDto>(notification));
        }
    }
}