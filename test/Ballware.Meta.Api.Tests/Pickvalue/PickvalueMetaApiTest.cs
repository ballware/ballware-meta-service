using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Pickvalue;

public class PickvalueMetaApiTest : ApiMappingBaseTest
{   
    [Test]
    public async Task HandleSelectListForEntityAndField_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedField = "field1";
        var expectedList = new List<PickvalueSelectEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Value = 1,
                Name = "Name 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Value = 2,
                Name = "Name 2"
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<IPickvalueMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListForEntityFieldAsync(expectedTenantId, expectedEntity, expectedField))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IPickvalueMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPickvalueMetaApi("pickvalue");
            });
        });
        
        // Act
        var response = await client.GetAsync($"pickvalue/selectlistforentityandfield/{expectedEntity}/{expectedField}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<PickvalueSelectEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
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
        var expectedEntity = "entity1";
        var expectedField = "field1";
        var expectedEntry = new PickvalueSelectEntry()
        {
            Id = Guid.NewGuid(),
            Value = 5,
            Name = "Value 5"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<IPickvalueMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectByValueAsync(expectedTenantId, expectedEntity, expectedField, expectedEntry.Value))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IPickvalueMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPickvalueMetaApi("pickvalue");
            });
        });
        
        // Act
        var response = await client.GetAsync($"pickvalue/selectbyvalueforentityandfield/{expectedEntity}/{expectedField}/{expectedEntry.Value}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<PickvalueSelectEntry>(await response.Content.ReadAsStringAsync());
        
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
        var notFoundResponse = await client.GetAsync($"pickvalue/selectbyvalueforentityandfield/{expectedEntity}/{expectedField}/99");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}