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

namespace Ballware.Meta.Api.Tests.Tenant;

public class TenantMetaApiTest : ApiMappingBaseTest
{   
    [Test]
    public async Task HandleMetadataForTenant_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();

        var providedEntry = new Data.Public.Tenant()
        {
            Id = expectedTenantId,
            Name = "Tenant One",
            Navigation = "{}",
            RightsCheckScript = null
        };
        
        var expectedEntry = new MetaTenant()
        {
            Id = providedEntry.Id,
            Name = "Tenant One",
            Navigation = "{}",
            RightsCheckScript = null
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(providedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantMetaApi("tenant");
            });
        });
        
        // Act
        var response = await client.GetAsync($"tenant/metadatafortenant/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<MetaTenant>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        // Act
        var notFoundResponse = await client.GetAsync($"tenant/metadatafortenant/{Guid.NewGuid()}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
        
        // Arrange
        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(Guid.NewGuid());    
        
        // Act
        var forbidResponse = await client.GetAsync($"tenant/metadatafortenant/{expectedEntry.Id}");
        
        // Assert
        Assert.That(forbidResponse.StatusCode,Is.EqualTo(HttpStatusCode.Forbidden));
    }
    
    [Test]
    public async Task HandleSelectList_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedList = new List<TenantSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 2"
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListAsync())
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantMetaApi("tenant");
            });
        });
        
        // Act
        var response = await client.GetAsync($"tenant/selectlist");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<TenantSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
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
        var expectedEntry = new TenantSelectListEntry()
        {
            Id = Guid.NewGuid(),
            Name = "Tenant 1"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectByIdAsync(expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantMetaApi("tenant");
            });
        });
        
        // Act
        var response = await client.GetAsync($"tenant/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<TenantSelectListEntry>(await response.Content.ReadAsStringAsync());
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine));
        });

        // Arrange
        repositoryMock
            .Setup(r => r.SelectByIdAsync(expectedEntry.Id))
            .ReturnsAsync(null as TenantSelectListEntry);
        
        // Act
        var notFoundResponse = await client.GetAsync($"tenant/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleAllowedTenantsForUser_succeeds()
    {
        // Arrange
        var expectedClaims = new Dictionary<string, object>() { };
        var expectedEntries = new List<TenantSelectListEntry>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 2"
            }
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserClaims(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedClaims);
        
        repositoryMock
            .Setup(r => r.AllowedTenantsAsync(expectedClaims))
            .ReturnsAsync(expectedEntries);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantMetaApi("tenant");
            });
        });
        
        // Act
        var response = await client.GetAsync($"tenant/allowed");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<TenantSelectListEntry>>(await response.Content.ReadAsStringAsync());
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries, result, TestContext.WriteLine));
        });
    }
}