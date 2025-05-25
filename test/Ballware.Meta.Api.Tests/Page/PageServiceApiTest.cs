using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Tests.Utils;
using Microsoft.AspNetCore.Builder;

namespace Ballware.Meta.Api.Tests.Page;

public class PageServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMapper_succeeds()
    {
        // Arrange
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPageServiceApi("page");
            });
        });
        
        Assert.That(client, Is.Not.Null);
    }
}