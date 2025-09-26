using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Export;

public class ExportServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleFetchById_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Export()
        {
            Id = Guid.NewGuid(),
            Application = "meta",
            Entity = "entity",
            Query = "primary",
            ExpirationStamp = DateTime.Now,
            MediaType = "application/json"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IExportMetaRepository>();

        repositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IExportMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapExportServiceApi("export");
            });
        });
        
        var response = await client.GetAsync($"export/exportbyidfortenant/{expectedTenantId}/{expectedEntry.Id}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Export>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"export/exportbyidfortenant/{expectedTenantId}/{Guid.NewGuid()}");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleCreateForTenantBehalfOfUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Export()
        {
            Id = Guid.NewGuid(),
            Application = "meta",
            Entity = "entity",
            Query = "primary",
            ExpirationStamp = DateTime.Now,
            MediaType = "application/json"
        };

        var repositoryMock = new Mock<IExportMetaRepository>();

        repositoryMock
            .Setup(r => r.NewAsync(expectedTenantId, "primary", It.IsAny<IDictionary<string, object>>()))
            .ReturnsAsync(new Data.Public.Export()
            {
                Id = Guid.NewGuid(),
            });
        
        repositoryMock
            .Setup(r => r.SaveAsync(expectedTenantId, expectedUserId, "primary",
                It.IsAny<IDictionary<string, object>>(), It.IsAny<Data.Public.Export>()))
            .Callback<Guid, Guid?, string, IDictionary<string, object>, Data.Public.Export>((tenantId, userId, identifier, claims, content) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tenantId, Is.EqualTo(expectedTenantId));
                    Assert.That(userId, Is.EqualTo(expectedUserId));
                    Assert.That(identifier, Is.EqualTo("primary"));
                    Assert.That(DeepComparer.AreEqual(content, expectedEntry, TestContext.WriteLine));
                });
                
            });

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IExportMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapExportServiceApi("export");
            });
        });
        
        var response = await client.PostAsync($"export/createexportfortenantbehalfofuser/{expectedTenantId}/{expectedUserId}", 
            JsonContent.Create(new ExportCreatePayload()
            {
                Application = expectedEntry.Application,
                Entity = expectedEntry.Entity,
                Query = expectedEntry.Query,
                ExpirationStamp = expectedEntry.ExpirationStamp,
                MediaType = expectedEntry.MediaType,
            }));
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));

        repositoryMock.Verify(r => r.SaveAsync(
            expectedTenantId,
            expectedUserId,
            "primary",
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<Data.Public.Export>()), Times.Once);
    }
}