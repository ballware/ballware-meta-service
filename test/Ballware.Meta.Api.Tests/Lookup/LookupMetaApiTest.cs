using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Lookup;

public class LookupMetaApiTest : ApiMappingBaseTest
{   
    [Test]
    public async Task HandleSelectList_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedList = new List<LookupSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Identifier = "lookup1",
                Name = "Lookup 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Identifier = "lookup2",
                Name = "Lookup 2"
            }
        };

        var mapsterConfig = new TypeAdapterConfig();

        new MetaApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<ILookupMetaRepository>();

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
            services.AddSingleton<ILookupMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapLookupMetaApi("lookup");
            });
        });
        
        // Act
        var response = await client.GetAsync($"lookup/selectlist");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<LookupSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
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
        var expectedEntry = new LookupSelectListEntry()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            Name = "Lookup 1"
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new MetaApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<ILookupMetaRepository>();

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
            services.AddSingleton<ILookupMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapLookupMetaApi("lookup");
            });
        });
        
        // Act
        var response = await client.GetAsync($"lookup/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<LookupSelectListEntry>(await response.Content.ReadAsStringAsync());
        
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
        var notFoundResponse = await client.GetAsync($"lookup/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}