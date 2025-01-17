using System;
using System.Net;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize("metaApi", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SubscriptionController : ControllerBase
{
    private ISubscriptionMetaRepository SubscriptionRepository { get; }

    public SubscriptionController(
        ISubscriptionMetaRepository subscriptionRepository)
    {
        SubscriptionRepository = subscriptionRepository;
    }

    [HttpGet]
    [Route("subscriptionmetadatabytenantandid/{tenant}/{id}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query subscription metadata by tenant and id",
      Description = "",
      OperationId = "MetadataForSubscriptionByTenantAndId"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Notification metadata", typeof(Subscription), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SubscriptionMetadataByTenantAndId(Guid tenant, Guid id)
    {
        var subscription = await SubscriptionRepository.MetadataByTenantAndIdAsync(tenant, id);

        return Ok(subscription);
    }

    [HttpGet]
    [Route("activesubscriptionsforfrequency/{frequency}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query active subscriptions by frequency",
      Description = "",
      OperationId = "ActiveSubscriptionsByFrequency"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of active subscriptions", typeof(IEnumerable<Subscription>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> ActiveSubscriptionsForTenantAndFrequency(int frequency)
    {
        var activesubscriptionsforfrequency = await SubscriptionRepository.GetActiveSubscriptionsByFrequencyAsync(frequency);

        return Ok(activesubscriptionsforfrequency);
    }

    [HttpPost]
    [Route("setsendresult/{tenant}/{id}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Send send result for subscription",
      Description = "",
      OperationId = "SetSendResultForSubscription"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Set send result successful")]
    public async Task<IActionResult> SetSendResultForSubscription(Guid tenant, Guid id, [FromBody] string error)
    {
        await SubscriptionRepository.SetLastErrorAsync(tenant, id, error);

        return Ok();
    }
}
