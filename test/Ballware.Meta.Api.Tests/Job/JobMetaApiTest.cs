using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Shared.Authorization;
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Job;

public class JobMetaApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandlePendingJobsForUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedList = new List<Data.Public.Job>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Scheduler = "meta-test",
                Identifier = "import",
                Owner = expectedUserId,
                State = JobStates.Queued
            },
            new()
            {
                Id = Guid.NewGuid(),
                Scheduler = "meta-test",
                Identifier = "export",
                Owner = expectedUserId,
                State = JobStates.Queued
            }
        };
        
        var mapsterConfig = new TypeAdapterConfig();

        new MetaApiProfile().Register(mapsterConfig);

        mapsterConfig.Compile();
        
        var mapper = new Mapper(mapsterConfig);

        var principalUtilsMock = new Mock<IPrincipalUtils>();
        var tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        var repositoryMock = new Mock<IJobMetaRepository>();

        principalUtilsMock
            .Setup(p => p.GetUserTenandId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedTenantId);
        
        principalUtilsMock
            .Setup(p => p.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(expectedUserId);
        
        repositoryMock
            .Setup(r => r.PendingJobsForUser(expectedTenantId, expectedUserId))
            .ReturnsAsync(expectedList);

        var client = await CreateApplicationClientAsync("metaApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IPrincipalUtils>(principalUtilsMock.Object);
            services.AddSingleton<ITenantRightsChecker>(tenantRightsCheckerMock.Object);
            services.AddSingleton<IJobMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapJobMetaApi("job");
            });
        });
        
        // Act
        var response = await client.GetAsync($"job/pendingjobsforuser");
        
        // Assert
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<IEnumerable<Data.Public.Job>>(await response.Content.ReadAsStringAsync());

        Assert.That(DeepComparer.AreListsEqual(expectedList, result, TestContext.WriteLine));
    }
}