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
                        Query = "fake pickvalue query"
                    },
                    new ()
                    {
                        Name = "ProcessingState",
                        Query = "fake processingstate query"
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
                        Query = "fake entity identifier query"
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
        var notificationMetaRepository = new Mock<INotificationMetaRepository>();
        var pageMetaRepository = new Mock<IPageMetaRepository>();
        var statisticMetaRepository = new Mock<IStatisticMetaRepository>();
        var subscriptionMetaRepository = new Mock<ISubscriptionMetaRepository>();
        var pickvalueMetaRepository = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepository = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        metaDbConnectionFactory
            .Setup(f => f.ConnectionString)
            .Returns(expectedMetaConnectionString);
        
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
        
        var response = await client.GetAsync($"tenant/reportmetadatasourcesfortenant/{expectedTenantId}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ServiceTenantReportDatasourceDefinition>>(await response.Content.ReadAsStringAsync());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries, result, TestContext.WriteLine), Is.True);
        });
    }
}