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
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Pickvalue;

public class PickvalueServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleSelectByValueForTenantEntityAndField_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedField = "field1";
        var expectedEntry = new PickvalueSelectEntry()
        {
            Id = Guid.NewGuid(),
            Value = 5,
            Name = "Name 5"
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IPickvalueMetaRepository>();

        repositoryMock
            .Setup(r => r.SelectByValueAsync(expectedTenantId, expectedEntity, expectedField, expectedEntry.Value))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPickvalueMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPickvalueServiceApi("pickvalue");
            });
        });
        
        var response = await client.GetAsync($"pickvalue/selectbyvaluefortenantandentityandfield/{expectedTenantId}/{expectedEntity}/{expectedField}/{expectedEntry.Value}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<PickvalueSelectEntry>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"pickvalue/selectbyvaluefortenantandentityandfield/{expectedTenantId}/{expectedEntry}/{expectedField}/99");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}