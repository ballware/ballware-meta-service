using System.Net;
using System.Text.Json;
using AutoMapper;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Api.Mappings;
using Ballware.Meta.Api.Tests.Utils;
using Ballware.Meta.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.Statistic;

public class StatisticServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMetadataForTenantAndIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.Statistic()
        {
            Id = Guid.NewGuid(),
            Identifier = "statisticOne",
            Name = "Statistic One",
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var repositoryMock = new Mock<IStatisticMetaRepository>();

        repositoryMock
            .Setup(r => r.MetadataByIdentifierAsync(expectedTenantId, expectedEntry.Identifier))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IStatisticMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapStatisticServiceApi("statistic");
            });
        });
        
        var response = await client.GetAsync($"statistic/metadatafortenantandidentifier/{expectedTenantId}/{expectedEntry.Identifier}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.Statistic>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var notFoundResponse = await client.GetAsync($"statistic/metadatafortenantandidentifier/{expectedTenantId}/unknownidentifier");
        
        Assert.That(notFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
}