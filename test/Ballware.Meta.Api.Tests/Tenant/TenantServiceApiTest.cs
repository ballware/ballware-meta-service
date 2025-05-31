using System.Net;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Tenant;

public class TenantServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleServiceMetadataForTenant_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var providedEntry = new Data.Public.Tenant()
        {
            Id = expectedTenantId,
            Provider = "mssql",
            Name = "Tenant One",
        };
        
        var expectedEntry = new ServiceTenant()
        {
            Id = providedEntry.Id,
            Provider = providedEntry.Provider,
            Name = providedEntry.Name,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var metaDbConnectionFactory = new Mock<IMetaDbConnectionFactory>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var lookupMetaRepositoryMock = new Mock<ILookupMetaRepository>();
        var documentMetaRepositoryMock = new Mock<IDocumentMetaRepository>();
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var mlModelMetaRepositoryMock = new Mock<IMlModelMetaRepository>();
        var notificationMetaRepository = new Mock<INotificationMetaRepository>();
        var pageMetaRepository = new Mock<IPageMetaRepository>();
        var statisticMetaRepository = new Mock<IStatisticMetaRepository>();
        var subscriptionMetaRepository = new Mock<ISubscriptionMetaRepository>();
        var pickvalueMetaRepository = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepository = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        repositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(providedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IMetaDbConnectionFactory>(metaDbConnectionFactory.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<ILookupMetaRepository>(lookupMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentMetaRepository>(documentMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IMlModelMetaRepository>(mlModelMetaRepositoryMock.Object);
            services.AddSingleton<INotificationMetaRepository>(notificationMetaRepository.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepository.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepository.Object);
            services.AddSingleton<ISubscriptionMetaRepository>(subscriptionMetaRepository.Object);
            services.AddSingleton<IPickvalueMetaRepository>(pickvalueMetaRepository.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(processingStateMetaRepository.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantServiceApi("tenant");
            });
        });
        
        var response = await client.GetAsync($"tenant/servicemetadatafortenant/{expectedTenantId}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<ServiceTenant>(await response.Content.ReadAsStringAsync());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(expectedEntry.Id));
            Assert.That(result?.Name, Is.EqualTo(expectedEntry.Name));
        });
        
        var notFoundResponse = await client.GetAsync($"tenant/servicemetadatafortenant/{Guid.NewGuid()}");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleReportMetaDatasourcesForTenant_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedMetaConnectionString = "fake-connection-string";
        
        var expectedEntries = new List<ServiceTenantReportDatasourceDefinition>() {
            new()
            {
                Name = "Meta",
                ConnectionString = expectedMetaConnectionString,
                Tables =
                [
                    new ()
                    {
                        Name = "Pickvalue",
                        Query = "fake entity pickvalue query"
                    },
                    new ()
                    {
                        Name = "ProcessingState",
                        Query = "fake entity state query"
                    }
                ]
            },
            new()
            {
                Name = "MetaLookups",
                ConnectionString = expectedMetaConnectionString,
                Tables =
                [
                    new ()
                    {
                        Name = "documentLookup",
                        Query = "fake document query"
                    },
                    new ()
                    {
                        Name = "documentationLookup",
                        Query = "fake documentation query"
                    },
                    new ()
                    {
                        Name = "entityLookup",
                        Query = "fake entity query"
                    },
                    new ()
                    {
                        Name = "lookupLookup",
                        Query = "fake lookup query"
                    },
                    new ()
                    {
                        Name = "mlmodelLookup",
                        Query = "fake mlmodel query"
                    },
                    new ()
                    {
                        Name = "notificationLookup",
                        Query = "fake notification query"
                    },
                    new ()
                    {
                        Name = "pageLookup",
                        Query = "fake page query"
                    },
                    new ()
                    {
                        Name = "statisticLookup",
                        Query = "fake statistic query"
                    },
                    new ()
                    {
                        Name = "subscriptionLookup",
                        Query = "fake subscription query"
                    },
                    new ()
                    {
                        Name = "tenantLookup",
                        Query = "fake tenant query"
                    },
                    new ()
                    {
                        Name = "entityIdentifierLookup",
                        Query = "fake entity query"
                    },
                    new ()
                    {
                        Name = "entityRightLookup",
                        Query = "fake entity right query"
                    },
                    new ()
                    {
                        Name = "entityStateLookup",
                        Query = "fake entity state query"
                    },
                    new ()
                    {
                        Name = "entityPickvalueLookup",
                        Query = "fake entity pickvalue query"
                    },
                ]
            },
            new()
            {
                Name = "Pickvalues",
                ConnectionString = expectedMetaConnectionString,
                Tables =
                [
                    new () 
                    {
                        Name = "Pickvalue_entity1_field2",
                        Query = "fake pickvalue entity1 field2 query"
                    },
                ]
            },
            new()
            {
                Name = "ProcessingStates",
                ConnectionString = expectedMetaConnectionString,
                Tables =
                [
                    new ()
                    {
                        Name = "ProcessingState_entity1",
                        Query = "fake state entity1 query"
                    }
                ]
            }
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var metaDbConnectionFactory = new Mock<IMetaDbConnectionFactory>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var lookupMetaRepositoryMock = new Mock<ILookupMetaRepository>();
        var documentMetaRepositoryMock = new Mock<IDocumentMetaRepository>();
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var mlModelMetaRepositoryMock = new Mock<IMlModelMetaRepository>();
        var notificationMetaRepositoryMock = new Mock<INotificationMetaRepository>();
        var pageMetaRepositoryMock = new Mock<IPageMetaRepository>();
        var statisticMetaRepositoryMock = new Mock<IStatisticMetaRepository>();
        var subscriptionMetaRepositoryMock = new Mock<ISubscriptionMetaRepository>();
        var pickvalueMetaRepositoryMock = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepositoryMock = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        metaDbConnectionFactory
            .Setup(f => f.ConnectionString)
            .Returns(expectedMetaConnectionString);
        
        documentMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake document query");
        
        documentationMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake documentation query");
        
        entityMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake entity query");
        
        lookupMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake lookup query");
        
        mlModelMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake mlmodel query");
        
        notificationMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake notification query");
        
        pageMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake page query");
        
        statisticMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake statistic query");
        
        subscriptionMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake subscription query");
        
        repositoryMock
            .Setup(r => r.GenerateListQueryAsync())
            .ReturnsAsync("fake tenant query");
        
        entityMetaRepositoryMock
            .Setup(r => r.GenerateRightsListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake entity right query");
        
        processingStateMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake entity state query");
        
        processingStateMetaRepositoryMock
            .Setup(r => r.GetProcessingStateAvailabilityAsync(expectedTenantId))
            .ReturnsAsync(["entity1"]);
        
        processingStateMetaRepositoryMock
            .Setup(r => r.GenerateAvailableQueryAsync(expectedTenantId, "entity1"))
            .ReturnsAsync("fake state entity1 query");
        
        pickvalueMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake entity pickvalue query");
        
        pickvalueMetaRepositoryMock
            .Setup(r => r.GetPickvalueAvailabilityAsync(expectedTenantId))
            .ReturnsAsync(new List<PickvalueAvailability>()
            {
                new ()
                {
                    Entity = "entity1",
                    Field = "field2"
                }
            });
        
        pickvalueMetaRepositoryMock
            .Setup(r => r.GenerateAvailableQueryAsync(expectedTenantId, "entity1", "field2"))
            .ReturnsAsync("fake pickvalue entity1 field2 query");
        
        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IMetaDbConnectionFactory>(metaDbConnectionFactory.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<ILookupMetaRepository>(lookupMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentMetaRepository>(documentMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IMlModelMetaRepository>(mlModelMetaRepositoryMock.Object);
            services.AddSingleton<INotificationMetaRepository>(notificationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepositoryMock.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepositoryMock.Object);
            services.AddSingleton<ISubscriptionMetaRepository>(subscriptionMetaRepositoryMock.Object);
            services.AddSingleton<IPickvalueMetaRepository>(pickvalueMetaRepositoryMock.Object);
            services.AddSingleton<IProcessingStateMetaRepository>(processingStateMetaRepositoryMock.Object);
            services.AddSingleton<ITenantMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTenantServiceApi("tenant");
            });
        });
        
        var response = await client.GetAsync($"tenant/reportmetadatasourcesfortenant/{expectedTenantId}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ServiceTenantReportDatasourceDefinition>>(await response.Content.ReadAsStringAsync());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(DeepComparer.AreListsEqual(expectedEntries, result, TestContext.WriteLine), Is.True);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries[0].Tables, result.ToList()[0].Tables, TestContext.WriteLine), Is.True);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries[1].Tables, result.ToList()[1].Tables, TestContext.WriteLine), Is.True);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries[2].Tables, result.ToList()[2].Tables, TestContext.WriteLine), Is.True);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries[3].Tables, result.ToList()[3].Tables, TestContext.WriteLine), Is.True);
        });
    }
}