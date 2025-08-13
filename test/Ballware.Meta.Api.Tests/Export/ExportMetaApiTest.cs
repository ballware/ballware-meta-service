using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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