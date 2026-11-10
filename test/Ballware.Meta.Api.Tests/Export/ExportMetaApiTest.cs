using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Tests.Utils;
using Microsoft.AspNetCore.Builder;

namespace Ballware.Meta.Api.Tests.Export;

public class ExportMetaApiTest : ApiMappingBaseTest
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
                endpoints.MapExportMetaApi("export");
            });
        });
        
        Assert.That(client, Is.Not.Null);
    }
}