using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Documentation;

public class DocumentationServiceApiTest : ApiMappingBaseTest
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
                endpoints.MapDocumentationServiceApi("documentation");
            });
        });
        
        Assert.That(client, Is.Not.Null);
    }
}