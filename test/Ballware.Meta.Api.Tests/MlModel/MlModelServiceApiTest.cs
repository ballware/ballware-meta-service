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
using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ballware.Meta.Api.Tests.MlModel;

public class MlModelServiceApiTest : ApiMappingBaseTest
{
    [Test]
    public async Task HandleMetadataByTenantAndId_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.MlModel()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            Type = MlModelTypes.Regression,
            TrainSql = "select * from lookup where TenantId=@tenantId"
        };
        
        var fakeTenant = new Data.Public.Tenant()
        {
            Id = expectedTenantId,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IMlModelMetaRepository>();

        tenantRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(fakeTenant);
        
        repositoryMock
            .Setup(r => r.MetadataByTenantAndIdAsync(expectedTenantId, expectedEntry.Id))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IMlModelMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMlModelServiceApi("mlmodel");
            });
        });
        
        var response = await client.GetAsync($"mlmodel/metadatabytenantandid/{expectedTenantId}/{expectedEntry.Id}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.MlModel>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var tenantNotFoundResponse = await client.GetAsync($"mlmodel/metadatabytenantandid/{Guid.NewGuid()}/{Guid.NewGuid()}");
        
        Assert.That(tenantNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
        
        var modelNotFoundResponse = await client.GetAsync($"mlmodel/metadatabytenantandid/{expectedTenantId}/{Guid.NewGuid()}");
        
        Assert.That(modelNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleMetadataByTenantAndIdentifier_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedEntry = new Data.Public.MlModel()
        {
            Id = Guid.NewGuid(),
            Identifier = "lookup1",
            Type = MlModelTypes.Regression,
            TrainSql = "select * from lookup where TenantId=@tenantId"
        };
        
        var fakeTenant = new Data.Public.Tenant()
        {
            Id = expectedTenantId,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IMlModelMetaRepository>();

        tenantRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(fakeTenant);
        
        repositoryMock
            .Setup(r => r.MetadataByTenantAndIdentifierAsync(expectedTenantId, expectedEntry.Identifier))
            .ReturnsAsync(expectedEntry);

        // Act
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IMlModelMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMlModelServiceApi("mlmodel");
            });
        });
        
        var response = await client.GetAsync($"mlmodel/metadatabytenantandidentifier/{expectedTenantId}/{expectedEntry.Identifier}");
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        var result = JsonSerializer.Deserialize<Data.Public.MlModel>(await response.Content.ReadAsStringAsync());
        
        Assert.That(DeepComparer.AreEqual(expectedEntry, result, TestContext.WriteLine), Is.True);
        
        var tenantNotFoundResponse = await client.GetAsync($"mlmodel/metadatabytenantandidentifier/{Guid.NewGuid()}/{expectedEntry.Identifier}");
        
        Assert.That(tenantNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
        
        var modelNotFoundResponse = await client.GetAsync($"mlmodel/metadatabytenantandidentifier/{expectedTenantId}/unknownidentifier");
        
        Assert.That(modelNotFoundResponse.StatusCode,Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task HandleSaveTrainingStateBehalfOfUser_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        
        var providedPayload = new MlModelTrainingState()
        {
            Id = Guid.NewGuid(),
            State = MlModelTrainingStates.UpToDate,
            Result = null
        };

        var fakeTenant = new Data.Public.Tenant()
        {
            Id = expectedTenantId,
        };
        
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServiceApiProfile>();
        });
        
        var mapper = mapperConfig.CreateMapper();

        var tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        var repositoryMock = new Mock<IMlModelMetaRepository>();

        tenantRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(fakeTenant);

        repositoryMock
            .Setup(r => r.SaveTrainingStateAsync(expectedTenantId, expectedUserId, It.IsAny<MlModelTrainingState>()))
            .Callback<Guid, Guid, MlModelTrainingState>((tenantId, userId, trainingState) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tenantId, Is.EqualTo(expectedTenantId));
                    Assert.That(userId, Is.EqualTo(expectedUserId));
                    Assert.That(DeepComparer.AreEqual(providedPayload, trainingState, TestContext.WriteLine), Is.True);
                });
            });
        
        var client = await CreateApplicationClientAsync("serviceApi", services =>
        {
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<ITenantMetaRepository>(tenantRepositoryMock.Object);
            services.AddSingleton<IMlModelMetaRepository>(repositoryMock.Object);
        }, app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMlModelServiceApi("mlmodel");
            });
        });

        // Act
        var response = await client.PostAsync($"mlmodel/savetrainingstatebehalfofuser/{expectedTenantId}/{expectedUserId}",
            JsonContent.Create(providedPayload));
        
        Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.OK));
        
        repositoryMock.Verify(r => r.SaveTrainingStateAsync(
            expectedTenantId,
            expectedUserId,
            It.IsAny<MlModelTrainingState>()), Times.Once);
        
    }
}