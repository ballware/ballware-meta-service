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
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.ProcessingState;

public class ProcessingStateServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleSelectListAllSuccessorsForTenantAndEntityByState_succeeds()
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
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

        repositoryMock
            .Setup(r => r.SelectListPossibleSuccessorsForEntityAsync(expectedTenantId, expectedEntity, 10))
            .ReturnsAsync(expectedList);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateServiceApi("processingstate");
            });
        });
        
        var response = await client.GetAsync($"processingstate/selectlistallsuccessorsfortenantandentitybystate/{expectedTenantId}/{expectedEntity}/10");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<ProcessingStateSelectListEntry>>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreListsEqual(expectedList, result, TestContext.WriteLine), Is.True);
    }
    
    [Test]
    public async Task HandleSelectByStateForTenantAndEntityByIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntity = "entity1";
        var expectedEntry = new ProcessingStateSelectListEntry()
        {
            Id = Guid.NewGuid(),
            State = 1,
            Name = "Name 1",
            Finished = false,
            Locked = false,
            ReasonRequired = false
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IProcessingStateMetaRepository>();

        repositoryMock
            .Setup(r => r.SelectByStateAsync(expectedTenantId, expectedEntity, expectedEntry.State))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IProcessingStateMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapProcessingStateServiceApi("processingstate");
            });
        });
        
        var response = await client.GetAsync($"processingstate/selectbystatefortenantandentity/{expectedTenantId}/{expectedEntity}/1");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<ProcessingStateSelectListEntry>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"processingstate/selectbystatefortenantandentity/{expectedTenantId}/{expectedEntity}/99");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}