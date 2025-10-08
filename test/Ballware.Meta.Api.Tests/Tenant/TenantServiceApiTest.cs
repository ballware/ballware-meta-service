using System.Net;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Public;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var pageMetaRepository = new Mock<IPageMetaRepository>();
        var statisticMetaRepository = new Mock<IStatisticMetaRepository>();
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
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepository.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepository.Object);
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
                Provider = "fake-provider",
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
                Provider = "fake-provider",
                ConnectionString = expectedMetaConnectionString,
                Tables =
                [
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
                Provider = "fake-provider",
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
                Provider = "fake-provider",
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
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var pageMetaRepositoryMock = new Mock<IPageMetaRepository>();
        var statisticMetaRepositoryMock = new Mock<IStatisticMetaRepository>();
        var pickvalueMetaRepositoryMock = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepositoryMock = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        metaDbConnectionFactory
            .Setup(f => f.ConnectionString)
            .Returns(expectedMetaConnectionString);
        
        metaDbConnectionFactory
            .Setup(f => f.Provider)
            .Returns("fake-provider");
        
        documentationMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake documentation query");
        
        entityMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake entity query");
        
        lookupMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake lookup query");
        
        pageMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake page query");
        
        statisticMetaRepositoryMock
            .Setup(r => r.GenerateListQueryAsync(expectedTenantId))
            .ReturnsAsync("fake statistic query");
        
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
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepositoryMock.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepositoryMock.Object);
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
    
    [Test]
    public async Task HandleReportAllowedTenants_succeeds()
    {
        // Arrange
        var expectedEntries = new List<TenantSelectListEntry>
        {
            new TenantSelectListEntry()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 1"
            },
            new TenantSelectListEntry()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 2"
            },
            new TenantSelectListEntry()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 3"
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
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var pageMetaRepository = new Mock<IPageMetaRepository>();
        var statisticMetaRepository = new Mock<IStatisticMetaRepository>();
        var pickvalueMetaRepository = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepository = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();

        repositoryMock
            .Setup(r => r.SelectListAsync())
            .ReturnsAsync(expectedEntries);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IMetaDbConnectionFactory>(metaDbConnectionFactory.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<ILookupMetaRepository>(lookupMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepository.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepository.Object);
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
        
        var response = await client.GetAsync($"tenant/reportallowedtenants");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<TenantSelectListEntry>>(await response.Content.ReadAsStringAsync())?.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(DeepComparer.AreListsEqual(expectedEntries, result, TestContext.WriteLine), Is.True);
        });
    }
    
    [Test]
    public async Task HandleReportLookupMetadataForTenantAndLookup_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        
        var expectedTenantLookupIdentifier = "fakeTenantLookup";
        var expectedTenantLookupId = Guid.NewGuid();
        var expectedMetaLookupIdentifier = "fakeMetaLookup";
        var expectedPickvalueIdentifier = "Pickvalue_Entity1_field2";
        var expectedProcessingStateIdentifier = "ProcessingState_Entity1";
        
        var expectedTenantLookupResult = new Dictionary<string, object>
        {
            { "lookupType", "tenantlookup" },
            { "lookupId", expectedTenantLookupId },
            { "lookupIdentifier", expectedTenantLookupIdentifier }
        };
        
        var expectedMetaLookupResult = new Dictionary<string, object>
        {
            { "lookupType", "metalookup" },
            { "lookupIdentifier", "fakeMetaLookup" }
        };
        
        var expectedProcessingStateResult = new Dictionary<string, object>
        {
            { "lookupType", "processingstate" },
            { "lookupEntity", "Entity1" }
        };
        
        var expectedPickvalueResult = new Dictionary<string, object>
        {
            { "lookupType", "pickvalue" },
            { "lookupEntity", "Entity1" },
            { "lookupField", "field2" }
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var metaDbConnectionFactory = new Mock<IMetaDbConnectionFactory>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var lookupMetaRepositoryMock = new Mock<ILookupMetaRepository>();
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var pageMetaRepositoryMock = new Mock<IPageMetaRepository>();
        var statisticMetaRepositoryMock = new Mock<IStatisticMetaRepository>();
        var pickvalueMetaRepositoryMock = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepositoryMock = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();
        
        lookupMetaRepositoryMock
            .Setup(r => r.ByIdentifierAsync(expectedTenantId, expectedTenantLookupIdentifier))
            .ReturnsAsync(new Data.Public.Lookup
            {
                Id = expectedTenantLookupId,
                Identifier = expectedTenantLookupIdentifier,
            });
        
        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IMetaDbConnectionFactory>(metaDbConnectionFactory.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<ILookupMetaRepository>(lookupMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepositoryMock.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepositoryMock.Object);
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
        
        var tenantLookupResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/lookups/{expectedTenantLookupIdentifier}");
        var metaLookupResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/metalookups/{expectedMetaLookupIdentifier}");
        var pickvalueResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/pickvalues/{expectedPickvalueIdentifier}");
        var processingStateResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/processingstates/{expectedProcessingStateIdentifier}");
        
        Assert.Multiple(() =>
        {
            Assert.That(tenantLookupResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
            Assert.That(metaLookupResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
            Assert.That(pickvalueResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
            Assert.That(processingStateResponse.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        });
        
        var tenantLookupResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(await tenantLookupResponse.Content.ReadAsStringAsync());
        var metaLookupResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(await metaLookupResponse.Content.ReadAsStringAsync());
        var pickvalueResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(await pickvalueResponse.Content.ReadAsStringAsync());
        var processingStateResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(await processingStateResponse.Content.ReadAsStringAsync());

        Assert.Multiple(() =>
        {
            Assert.That(tenantLookupResult, Is.Not.Null);
            Assert.That(metaLookupResult, Is.Not.Null);
            Assert.That(pickvalueResult, Is.Not.Null);
            Assert.That(processingStateResult, Is.Not.Null);

            Assert.That(tenantLookupResult.Count, Is.EqualTo(expectedTenantLookupResult.Count));
            
            foreach (var key in expectedTenantLookupResult.Keys)
            {
                Assert.That(tenantLookupResult, Does.ContainKey(key));
                Assert.That(tenantLookupResult[key], Is.EqualTo(expectedTenantLookupResult[key].ToString()));
            }
            
            Assert.That(metaLookupResult, Is.EqualTo(expectedMetaLookupResult));
            Assert.That(pickvalueResult, Is.EqualTo(expectedPickvalueResult));
            Assert.That(processingStateResult, Is.EqualTo(expectedProcessingStateResult));
        });
    }
    
    [Test]
    public async Task HandleReportLookupMetadataForTenantAndLookup_notFound()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var metaDbConnectionFactory = new Mock<IMetaDbConnectionFactory>();
        var entityMetaRepositoryMock = new Mock<IEntityMetaRepository>();
        var lookupMetaRepositoryMock = new Mock<ILookupMetaRepository>();
        var documentationMetaRepositoryMock = new Mock<IDocumentationMetaRepository>();
        var pageMetaRepositoryMock = new Mock<IPageMetaRepository>();
        var statisticMetaRepositoryMock = new Mock<IStatisticMetaRepository>();
        var pickvalueMetaRepositoryMock = new Mock<IPickvalueMetaRepository>();
        var processingStateMetaRepositoryMock = new Mock<IProcessingStateMetaRepository>();
        var repositoryMock = new Mock<ITenantMetaRepository>();
        
        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IMetaDbConnectionFactory>(metaDbConnectionFactory.Object);
            services.AddSingleton<IEntityMetaRepository>(entityMetaRepositoryMock.Object);
            services.AddSingleton<ILookupMetaRepository>(lookupMetaRepositoryMock.Object);
            services.AddSingleton<IDocumentationMetaRepository>(documentationMetaRepositoryMock.Object);
            services.AddSingleton<IPageMetaRepository>(pageMetaRepositoryMock.Object);
            services.AddSingleton<IStatisticMetaRepository>(statisticMetaRepositoryMock.Object);
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
        
        var datasourceNotFoundResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/undefineddatasource/{Guid.NewGuid()}");
        var lookupNotFoundResponse = await client.GetAsync($"tenant/reportlookupmetadatafortenantdatasourceandidentifier/{expectedTenantId}/lookups/undefinedlookup");

        Assert.Multiple(() =>
        {
            Assert.That(datasourceNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(lookupNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
        });
    }
}