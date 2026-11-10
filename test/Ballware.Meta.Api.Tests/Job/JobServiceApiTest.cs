using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Job;

public class JobServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleCreateJobForTenantBehalfOfUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        
        var providedPayload = new JobCreatePayload()
        {
            Scheduler = "meta-test",
            Identifier = "import",
            Options = "{}"
        };

        var expectedEntry = new Data.Public.Job()
        {
            Id = Guid.NewGuid(),
            Scheduler = providedPayload.Scheduler,
            Identifier = providedPayload.Identifier,
            Options = providedPayload.Options,
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new ServiceApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var repositoryMock = new Mock<IJobMetaRepository>();
        
        repositoryMock
            .Setup(r => r.CreateJobAsync(expectedTenantId, expectedUserId, providedPayload.Scheduler, providedPayload.Identifier, providedPayload.Options))
            .ReturnsAsync(expectedEntry);

        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IJobMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapJobServiceApi("job");
            });
        });

        // Act
        var response = await client.PostAsync($"job/createjobfortenantbehalfofuser/{expectedTenantId}/{expectedUserId}",
            JsonContent.Create(providedPayload));
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Job>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
    }
    
    [Test]
    public async Task HandleUpdateJobForTenantBehalfOfUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        
        var providedPayload = new JobUpdatePayload()
        {
            Id = Guid.NewGuid(),
            State = JobStates.InProgress,
            Result = null
        };

        var expectedEntry = new Data.Public.Job()
        {
            Id = providedPayload.Id,
            Scheduler = "meta-test",
            Identifier = "import",
            Options = "{}",
            State = JobStates.InProgress
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new ServiceApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var repositoryMock = new Mock<IJobMetaRepository>();
        
        repositoryMock
            .Setup(r => r.UpdateJobAsync(expectedTenantId, expectedUserId, providedPayload.Id, providedPayload.State, providedPayload.Result))
            .ReturnsAsync(expectedEntry);
        
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IJobMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapJobServiceApi("job");
            });
        });

        // Act
        var response = await client.PostAsync($"job/updatejobfortenantbehalfofuser/{expectedTenantId}/{expectedUserId}",
            JsonContent.Create(providedPayload));
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Job>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
    
        repositoryMock.Verify(r => r.UpdateJobAsync(
            expectedTenantId,
            expectedUserId,
            expectedEntry.Id,
            expectedEntry.State,
            expectedEntry.Result), Times.Once);
    }
}