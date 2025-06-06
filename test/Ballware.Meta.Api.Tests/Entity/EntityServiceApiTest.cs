using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ballware.Meta.Api.Tests.Entity;

public class EntityServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMetadataByIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();

        var providedCustomFunctions = new List<Data.Public.EntityCustomFunction>()
        {
            new EntityCustomFunction()
            {
                Identifier = "importjson",
                Options = new EntityImportFunctionOptions()
                {
                    Format = "json"
                },
                Type = EntityCustomFunctionTypes.Import
            },
            new EntityCustomFunction()
            {
                Identifier = "exportjson",
                Options = new EntityExportFunctionOptions()
                {
                    Format = "json"
                },
                Type = EntityCustomFunctionTypes.Export
            }
        }; 
        
        var providedEntry = new Data.Public.EntityMetadata()
        {
            Id = Guid.NewGuid(),
            DisplayName = "My Entity",
            Entity = "entity1",
            CustomFunctions = JsonConvert.SerializeObject(providedCustomFunctions)
        };

        var expectedEntry = new ServiceEntity()
        {
            Id = providedEntry.Id,
            Entity = "entity1",
            CustomFunctions = new List<ServiceEntityCustomFunction>()
            {
                new ServiceEntityCustomFunction()
                {
                    Id = "importjson",
                    Options = new ServiceEntityCustomFunctionOptions()
                    {
                        Format = "json"
                    },
                    Type = EntityCustomFunctionTypes.Import
                },
                new ServiceEntityCustomFunction()
                {
                    Id = "exportjson",
                    Options = new ServiceEntityCustomFunctionOptions()
                    {
                        Format = "json"
                    },
                    Type = EntityCustomFunctionTypes.Export
                }
            }
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IEntityMetaRepository>();

        repositoryMock
            .Setup(r => r.ByEntityAsync(expectedTenantId, expectedEntry.Entity))
            .ReturnsAsync(providedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IEntityMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapEntityServiceApi("entity");
            });
        });
        
        var response = await client.GetAsync($"entity/servicemetadatafortenantbyidentifier/{expectedTenantId}/{providedEntry.Entity}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<ServiceEntity>(await response.Content.ReadAsStringAsync());
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id,Is.EqualTo(expectedEntry.Id));
            Assert.That(result?.Entity,Is.EqualTo(expectedEntry.Entity));
            Assert.That(DeepComparer.AreListsEqual(expectedEntry.CustomFunctions, result?.CustomFunctions, TestContext.WriteLine), Is.True);
        });
        
        var notFoundResponse = await client.GetAsync($"entity/servicemetadatafortenantbyidentifier/{expectedTenantId}/unknownentity");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}