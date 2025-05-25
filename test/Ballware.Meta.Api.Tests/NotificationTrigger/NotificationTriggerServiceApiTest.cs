using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.NotificationTrigger;

public class NotificationTriggerServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleCreateForTenantAndNotificationBehalfOfUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        
        var providedPayload = new NotificationTriggerCreatePayload()
        {
            NotificationId = Guid.NewGuid(),
            Params = null
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<INotificationTriggerMetaRepository>();
        
        repositoryMock
            .Setup(r => r.NewAsync(expectedTenantId, "primary",  It.IsAny<ImmutableDictionary<string, object>>()))
            .ReturnsAsync(new Data.Public.NotificationTrigger()
            {
                Id = Guid.NewGuid(),
            })
            .Callback((Guid tenantId, string identifier, IDictionary<string, object> claims) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tenantId, Is.EqualTo(expectedTenantId));
                    Assert.That(identifier, Is.EqualTo("primary"));
                });
            });
        
        repositoryMock
            .Setup(r => r.SaveAsync(expectedTenantId, expectedUserId, "primary", It.IsAny<IDictionary<string, object>>(), It.IsAny<Data.Public.NotificationTrigger>()))
            .Callback((Guid tenantId, Guid? userId, string identifier, IDictionary<string, object> claims, Data.Public.NotificationTrigger payload) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tenantId, Is.EqualTo(expectedTenantId));
                    Assert.That(userId, Is.EqualTo(expectedUserId));
                    Assert.That(identifier, Is.EqualTo("primary"));
                    Assert.That(payload.NotificationId, Is.EqualTo(providedPayload.NotificationId));
                    Assert.That(payload.Params, Is.EqualTo(providedPayload.Params));
                });
            });

        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<INotificationTriggerMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationTriggerServiceApi("notificationtrigger");
            });
        });

        // Act
        var response = await client.PostAsync($"notificationtrigger/createnotificationtriggerfortenantbehalfofuser/{expectedTenantId}/{expectedUserId}",
            JsonContent.Create(providedPayload));
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));

        repositoryMock.Verify(r => r.NewAsync(
            expectedTenantId,
            "primary",
            It.IsAny<IDictionary<string, object>>()), Times.Once);
        
        repositoryMock.Verify(r => r.SaveAsync(
            expectedTenantId,
            expectedUserId,
            "primary",
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<Data.Public.NotificationTrigger>()), Times.Once);
    }
}