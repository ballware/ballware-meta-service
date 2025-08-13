using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
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

namespace Ballware.Meta.Api.Tests.Documentation;

public class DocumentationMetaApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleForEntity_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Documentation()
        {
            Id = Guid.NewGuid(),
            Entity = "entity1",
            Field = null,
            Content = "My test content"
        };

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IDocumentationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.ByEntityAndFieldAsync(expectedTenantId, expectedEntry.Entity, string.Empty))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDocumentationMetaApi("documentation");
            });
        });
        
        // Act
        var response = await client.GetAsync($"documentation/documentationforentity/{expectedEntry.Entity}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = await response.Content.ReadAsStringAsync();
        
        Assert.That(result, Is.EqualTo(expectedEntry.Content));
        
        // Act
        var emptyResponse = await client.GetAsync($"documentation/documentationforentity/fakeentity");
        
        // Assert
        Assert.That(emptyResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var emptyResult = await emptyResponse.Content.ReadAsStringAsync();
        
        Assert.That(emptyResult, Is.EqualTo(string.Empty));
    }
    
    [Test]
    public async Task HandleForEntityAndField_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Documentation()
        {
            Id = Guid.NewGuid(),
            Entity = "entity1",
            Field = "Field1",
            Content = "My test content"
        };

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IDocumentationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.ByEntityAndFieldAsync(expectedTenantId, expectedEntry.Entity, expectedEntry.Field))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDocumentationMetaApi("documentation");
            });
        });
        
        // Act
        var response = await client.GetAsync($"documentation/documentationforentityandfield/{expectedEntry.Entity}/{expectedEntry.Field}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = await response.Content.ReadAsStringAsync();
        
        Assert.That(result, Is.EqualTo(expectedEntry.Content));
        
        // Act
        var emptyResponse = await client.GetAsync($"documentation/documentationforentityandfield/{expectedEntry.Entity}/fakefield");
        
        // Assert
        Assert.That(emptyResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var emptyResult = await emptyResponse.Content.ReadAsStringAsync();
        
        Assert.That(emptyResult, Is.EqualTo(string.Empty));
    }
    
    [Test]
    public async Task HandleSelectList_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedList = new List<DocumentationSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Entity = "Entity1",
                Field = "Field1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Entity = "Entity1",
                Field = "Field2"
            }
        };

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IDocumentationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListForTenantAsync(expectedTenantId))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDocumentationMetaApi("documentation");
            });
        });
        
        // Act
        var response = await client.GetAsync($"documentation/selectlist");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<DocumentationSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
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
        var expectedEntry = new DocumentationSelectListEntry()
        {
            Id = Guid.NewGuid(),
            Entity = "Entity1",
            Field = "Field1"
        };

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IDocumentationMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectByIdForTenantAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDocumentationMetaApi("documentation");
            });
        });
        
        // Act
        var response = await client.GetAsync($"documentation/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<DocumentationSelectListEntry>(await response.Content.ReadAsStringAsync());
        
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
        var notFoundResponse = await client.GetAsync($"documentation/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}