using System.Net;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Lookup;

public class LookupServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMetadataForTenantAndId_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Lookup()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            ListQuery = "select * from lookup where TenantId=@tenantId",
            ByIdQuery = "select * from lookup where TenantId=@tenantId and Uuid=@id",
            Name = "My lookup",
            HasParam = false
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new ServiceApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var repositoryMock = new Mock<ILookupMetaRepository>();

        repositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ILookupMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapLookupServiceApi("lookup");
            });
        });
        
        var response = await client.GetAsync($"lookup/lookupmetadatabytenantandid/{expectedTenantId}/{expectedEntry.Id}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Lookup>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"lookup/lookupmetadatabytenantandid/{expectedTenantId}/{Guid.NewGuid()}");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleMetadataForTenantAndIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Lookup()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            ListQuery = "select * from lookup where TenantId=@tenantId",
            ByIdQuery = "select * from lookup where TenantId=@tenantId and Uuid=@id",
            Name = "My lookup",
            HasParam = false
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new ServiceApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var repositoryMock = new Mock<ILookupMetaRepository>();

        repositoryMock
            .Setup(r => r.ByIdentifierAsync(expectedTenantId, expectedEntry.Identifier))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ILookupMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapLookupServiceApi("lookup");
            });
        });
        
        var response = await client.GetAsync($"lookup/lookupmetadatabytenantandidentifier/{expectedTenantId}/{expectedEntry.Identifier}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Lookup>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"lookup/lookupmetadatabytenantandidentifier/{expectedTenantId}/unknownidentifier");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleMetadataForTenant_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntries = new List<Data.Public.Lookup>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Identifier = "lookup1",
                ListQuery = "select * from lookup where TenantId=@tenantId",
                ByIdQuery = "select * from lookup where TenantId=@tenantId and Uuid=@id",
                Name = "My lookup",
                HasParam = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Identifier = "lookup2",
                
                ListQuery = "select * from lookup where TenantId=@tenantId",
                ByIdQuery = "select * from lookup where TenantId=@tenantId and Uuid=@id",
                Name = "My lookup 2",
                HasParam = false
            }
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new ServiceApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var repositoryMock = new Mock<ILookupMetaRepository>();

        repositoryMock
            .Setup(r => r.AllForTenantAsync(expectedTenantId))
            .ReturnsAsync(expectedEntries);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ILookupMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapLookupServiceApi("lookup");
            });
        });
        
        var response = await client.GetAsync($"lookup/lookupmetadatabytenant/{expectedTenantId}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<Data.Public.Lookup>>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreListsEqual(expectedEntries, result, TestContext.WriteLine), Is.True);
    }
}