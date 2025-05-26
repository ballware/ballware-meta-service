using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Tests.Utils;
using Microsoft.AspNetCore.Builder;

namespace Ballware.Meta.Api.Tests.NotificationTrigger;

public class NotificationTriggerMetaApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMapper_succeeds()
    {
        // Arrange
        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationTriggerMetaApi("notificationtrigger");
            });
        });
        
        Assert.That(client, Is.Not.Null);
    }
}