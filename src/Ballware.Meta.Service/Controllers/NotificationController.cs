using System;
using System.Net;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Repository;
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
        private INotificationMetaRepository NotificationMetaRepository { get; }

        public NotificationController(
            INotificationMetaRepository notificationMetaRepository)
        {
            NotificationMetaRepository = notificationMetaRepository;
        }

        [HttpGet]
        [Route("notificationmetadatabytenantandid/{tenant}/{id}")]
        [Authorize("documentApi")]
        [ApiExplorerSettings(GroupName = "document")]
        [SwaggerOperation(
          Summary = "Query notification metadata by tenant and id",
          Description = "",
          OperationId = "MetadataForNotificationByTenantAndId"
        )]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Notification metadata", typeof(Notification), new[] { MimeMapping.KnownMimeTypes.Json })]
        public async Task<IActionResult> NotificationMetadataByTenantAndId(Guid tenant, Guid id)
        {
            var notification = await NotificationMetaRepository.MetadataByTenantAndIdAsync(tenant, id);

            return Ok(notification);
        }
    }
}