using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Notification;

public class NotificationServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMetadataByTenantAndId_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Notification()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            Name = "Notification name",
            DocumentId = Guid.NewGuid(),
            Params = null,
            State = 1,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<INotificationMetaRepository>();

        repositoryMock
            .Setup(r => r.MetadataByTenantAndIdAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<INotificationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationServiceApi("notification");
            });
        });
        
        var response = await client.GetAsync($"notification/notificationmetadatabytenantandid/{expectedTenantId}/{expectedEntry.Id}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Notification>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"notification/notificationmetadatabytenantandid/{expectedTenantId}/{Guid.NewGuid()}");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleMetadataByTenantAndIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Notification()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            Name = "Notification name",
            DocumentId = Guid.NewGuid(),
            Params = null,
            State = 1,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<INotificationMetaRepository>();

        repositoryMock
            .Setup(r => r.MetadataByTenantAndIdentifierAsync(expectedTenantId, expectedEntry.Identifier))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<INotificationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationServiceApi("notification");
            });
        });
        
        var response = await client.GetAsync($"notification/notificationmetadatabytenantandidentifier/{expectedTenantId}/{expectedEntry.Identifier}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Notification>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"notification/notificationmetadatabytenantandidentifier/{expectedTenantId}/unknownidentifier");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}