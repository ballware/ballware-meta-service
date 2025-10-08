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

namespace Ballware.Meta.Api.Tests.ProcessingState;

public class ProcessingStateMetaApiTest : ApiMappingBaseTest
{   
    [Test]
    public async Task HandleSelectList_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedList = new List<ProcessingStateSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                State = 1,
                Name = "Name 1",
                Finished = false,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 2,
                Name = "Name 2",
                Finished = true,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 3,
                Name = "Name 3",
                Finished = true,
                Locked = true,
                ReasonRequired = false
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var entityRightsCheckerMock = new Mock<IEntityRightsChecker>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

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
            services.AddSingleton<IEntityRightsChecker>(entityRightsCheckerMock.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateMetaApi("processingstate");
            });
        });
        
        // Act
        var response = await client.GetAsync($"processingstate/selectlist");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ProcessingStateSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
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
        var expectedEntry = new ProcessingStateSelectListEntry()
        {
            Id = Guid.NewGuid(),
            State = 5,
            Name = "Value 5"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var entityRightsCheckerMock = new Mock<IEntityRightsChecker>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

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
            services.AddSingleton<IEntityRightsChecker>(entityRightsCheckerMock.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateMetaApi("processingstate");
            });
        });
        
        // Act
        var response = await client.GetAsync($"processingstate/selectbyid/{expectedEntry.Id}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<ProcessingStateSelectListEntry>(await response.Content.ReadAsStringAsync());
        
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
        var notFoundResponse = await client.GetAsync($"processingstate/selectbyid/{Guid.NewGuid()}");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleSelectListForEntityByIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedList = new List<ProcessingStateSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                State = 1,
                Name = "Name 1",
                Finished = false,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 2,
                Name = "Name 2",
                Finished = true,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 3,
                Name = "Name 3",
                Finished = true,
                Locked = true,
                ReasonRequired = false
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var entityRightsCheckerMock = new Mock<IEntityRightsChecker>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListForEntityAsync(expectedTenantId, expectedEntity))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IEntityRightsChecker>(entityRightsCheckerMock.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateMetaApi("processingstate");
            });
        });
        
        // Act
        var response = await client.GetAsync($"processingstate/selectlistforentity/{expectedEntity}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ProcessingStateSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedList, result, TestContext.WriteLine));
        });
    }
    
    [Test]
    public async Task HandleSelectByStateForEntityByIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedEntry = new ProcessingStateSelectListEntry()
        {
            Id = Guid.NewGuid(),
            State = 5,
            Name = "Value 5"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var entityRightsCheckerMock = new Mock<IEntityRightsChecker>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectByStateAsync(expectedTenantId, expectedEntity, expectedEntry.State))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IEntityRightsChecker>(entityRightsCheckerMock.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateMetaApi("processingstate");
            });
        });
        
        // Act
        var response = await client.GetAsync($"processingstate/selectbystateforentity/{expectedEntity}/{expectedEntry.State}");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<ProcessingStateSelectListEntry>(await response.Content.ReadAsStringAsync());
        
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
        var notFoundResponse = await client.GetAsync($"processingstate/selectbystateforentity/{expectedEntity}/99");
        
        // Assert
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleSelectListAllSuccessorsForEntityByIdentifierAndState_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedList = new List<ProcessingStateSelectListEntry>
        {
            new()
            {
                Id = Guid.NewGuid(),
                State = 1,
                Name = "Name 1",
                Finished = false,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 2,
                Name = "Name 2",
                Finished = true,
                Locked = false,
                ReasonRequired = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                State = 3,
                Name = "Name 3",
                Finished = true,
                Locked = true,
                ReasonRequired = false
            }
        };

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MetaApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();
        
        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var entityRightsCheckerMock = new Mock<IEntityRightsChecker>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        repositoryMock
            .Setup(r => r.SelectListPossibleSuccessorsForEntityAsync(expectedTenantId, expectedEntity, 10))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IEntityRightsChecker>(entityRightsCheckerMock.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateMetaApi("processingstate");
            });
        });
        
        // Act
        var response = await client.GetAsync($"processingstate/selectlistallsuccessorsforentityandstate/{expectedEntity}/10");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ProcessingStateSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedList, result, TestContext.WriteLine));
        });
    }
}