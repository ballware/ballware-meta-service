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
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Notification;

public class NotificationMetaApiTest : ApiMappingBaseTest
{   
    [Test]
    public async Task HandleSelectList_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedList = new List<NotificationSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Notification 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Notification 2"
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<INotificationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListForTenantAsync(expectedTenantId))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<INotificationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationMetaApi("notification");
            });
        });
        
        // Act
        var response = await client.GetAsync($"notification/selectlist");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<NotificationSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedList, result, TestContext.WriteLine));
        });
    }
    
    [Test]
    public async Task HandleSelectById_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new NotificationSelectListEntry()
        {
            Id = Guid.NewGuid(),
            Name = "Notification 1"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<INotificationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectByIdForTenantAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<INotificationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapNotificationMetaApi("notification");
            });
        });
        
        // Act
        var response = await client.GetAsync($"notification/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<NotificationSelectListEntry>(await response.Content.ReadAsStringAsync());
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine));
        });

        // Arrange
        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(Guid.NewGuid());
        
        // Act
        var notFoundResponse = await client.GetAsync($"notification/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}