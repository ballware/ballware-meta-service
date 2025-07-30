using Ballware.Meta.Data.Common;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Jobs.Internal;
using Ballware.Shared.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Quartz;

namespace Ballware.Meta.Jobs.Tests;

public class FakeTenant
{
    public string Name { get; set; } = "TestEntity";
}

[TestFixture]
public class MetaImportJobTest
{
    private const string ExpectedFunctionIdentifier = "importjson";
    private Mock<IRepository<FakeTenant>> RepositoryMock { get; set; }
    private Mock<IJobMetaRepository> JobMetaRepositoryMock { get; set; }
    private Mock<ITenantMetaRepository> TenantMetaRepositoryMock { get; set; }
    private Mock<ITenantRightsChecker> TenantRightsCheckerMock { get; set; }
    private Mock<IJobsFileStorageAdapter> JobsFileStorageAdapterMock { get; set; }
    private Mock<IJobExecutionContext> JobExecutionContextMock { get; set; }

    private ServiceProvider ServiceProvider { get; set; }
    
    [SetUp]
    public void Setup()
    {
        RepositoryMock = new Mock<IRepository<FakeTenant>>();
        JobMetaRepositoryMock = new Mock<IJobMetaRepository>();
        TenantMetaRepositoryMock = new Mock<ITenantMetaRepository>();
        TenantRightsCheckerMock = new Mock<ITenantRightsChecker>();
        JobsFileStorageAdapterMock = new Mock<IJobsFileStorageAdapter>();
        
        JobExecutionContextMock = new Mock<IJobExecutionContext>();
        
        var triggerMock = new Mock<ITrigger>();
        
        triggerMock
            .Setup(trigger => trigger.JobKey)
            .Returns(JobKey.Create("import", "faketenant"));
        
        JobExecutionContextMock
            .Setup(c => c.Trigger)
            .Returns(triggerMock.Object);
        
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton<IRepository<FakeTenant>>(RepositoryMock.Object);
        
        ServiceProvider = serviceCollection
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        ServiceProvider.Dispose();
    }
    
    [Test]
    public async Task Execute_succeeds()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedJobId = Guid.NewGuid();
        var expectedTemporaryId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedClaims = new Dictionary<string, object>();
        
        var expectedTenant = new Tenant()
        {
            Id = expectedTenantId,
        };
        var expectedFileStream = new MemoryStream();
        var expectedEntity = new FakeTenant();
        
        TenantMetaRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(expectedTenant);
        
        JobsFileStorageAdapterMock
            .Setup(s => s.TemporaryFileByIdAsync(expectedTenantId, expectedTemporaryId))
            .ReturnsAsync(expectedFileStream);
        
        RepositoryMock
            .Setup(r => r.ImportAsync(
                expectedUserId,
                "importjson",
                expectedClaims,
                expectedFileStream,
                It.IsAny<Func<FakeTenant, Task<bool>>>()))
            .Returns(async (Guid? userId, string identifier, IDictionary<string, object> claims, Stream stream,
                Func<FakeTenant, Task<bool>> authorize) =>
            {
                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(userId, Is.EqualTo(expectedUserId));
                    Assert.That(identifier, Is.EqualTo(ExpectedFunctionIdentifier));
                    Assert.That(claims, Is.EqualTo(expectedClaims));
                    Assert.That(stream, Is.EqualTo(expectedFileStream));
                    Assert.That(await authorize(expectedEntity), Is.True);
                });
            });

        TenantRightsCheckerMock
            .Setup(c => c.HasRightAsync(expectedTenant, "meta", "faketenant", expectedClaims, ExpectedFunctionIdentifier))
            .ReturnsAsync(true);
        
        var jobDataMap = new JobDataMap
        {
            { "tenantId", expectedTenantId },
            { "jobId", expectedJobId },
            { "userId", expectedUserId },
            { "identifier", ExpectedFunctionIdentifier },
            { "claims", JsonConvert.SerializeObject(expectedClaims) },
            { "file", expectedTemporaryId }
        };

        JobExecutionContextMock
            .Setup(c => c.MergedJobDataMap)
            .Returns(jobDataMap);
        
        var job = new MetaImportJob<FakeTenant, IRepository<FakeTenant>>(
            ServiceProvider,
            JobMetaRepositoryMock.Object,
            TenantMetaRepositoryMock.Object,
            TenantRightsCheckerMock.Object,
            JobsFileStorageAdapterMock.Object);
        
        // Act
        await job.Execute(JobExecutionContextMock.Object);
        
        // Assert
        JobMetaRepositoryMock.Verify(
            r => r.UpdateJobAsync(expectedTenantId, expectedUserId, expectedJobId, JobStates.InProgress, string.Empty),
            Times.Once);
        
        JobsFileStorageAdapterMock.Verify(s => s.TemporaryFileByIdAsync(expectedTenantId, expectedTemporaryId), Times.Once);
        JobsFileStorageAdapterMock.Verify(s => s.RemoveTemporaryFileByIdBehalfOfUserAsync(expectedTenantId, expectedUserId, expectedTemporaryId), Times.Once);
        
        RepositoryMock.Verify(r => r.ImportAsync(
            expectedUserId,
            ExpectedFunctionIdentifier,
            expectedClaims,
            expectedFileStream,
            It.IsAny<Func<FakeTenant, Task<bool>>>()), Times.Once);
    }
    
    [Test]
    public void Execute_failed_unknown_tenant()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedJobId = Guid.NewGuid();
        var expectedTemporaryId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedClaims = new Dictionary<string, object>();
        
        TenantMetaRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(null as Tenant);
        
        var jobDataMap = new JobDataMap
        {
            { "tenantId", expectedTenantId },
            { "jobId", expectedJobId },
            { "userId", expectedUserId },
            { "identifier", ExpectedFunctionIdentifier },
            { "claims", JsonConvert.SerializeObject(expectedClaims) },
            { "file", expectedTemporaryId }
        };

        JobExecutionContextMock
            .Setup(c => c.MergedJobDataMap)
            .Returns(jobDataMap);
        
        var job = new MetaImportJob<FakeTenant, IRepository<FakeTenant>>(
            ServiceProvider,
            JobMetaRepositoryMock.Object,
            TenantMetaRepositoryMock.Object,
            TenantRightsCheckerMock.Object,
            JobsFileStorageAdapterMock.Object);
        
        // Act
        Assert.ThrowsAsync<JobExecutionException>(async () => await job.Execute(JobExecutionContextMock.Object), $"Tenant {expectedTenantId} unknown");
        
        // Assert
        JobMetaRepositoryMock.Verify(
            r => r.UpdateJobAsync(expectedTenantId, expectedUserId, expectedJobId, JobStates.InProgress, string.Empty),
            Times.Never);
        
        JobsFileStorageAdapterMock.Verify(s => s.TemporaryFileByIdAsync(expectedTenantId, expectedTemporaryId), 
            Times.Never);
        JobsFileStorageAdapterMock.Verify(s => s.RemoveTemporaryFileByIdBehalfOfUserAsync(expectedTenantId, expectedUserId, expectedTemporaryId), 
            Times.Never);
        
        RepositoryMock.Verify(r => r.ImportAsync(
            expectedUserId,
            ExpectedFunctionIdentifier,
            expectedClaims,
            It.IsAny<Stream>(),
            It.IsAny<Func<FakeTenant, Task<bool>>>()), 
            Times.Never);
    }
    
    [Test]
    public void Execute_failed_mandatory_parameter_missing()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedJobId = Guid.NewGuid();
        var expectedTemporaryId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedClaims = new Dictionary<string, object>();
        
        var expectedTenant = new Tenant()
        {
            Id = expectedTenantId,
        };
        
        TenantMetaRepositoryMock
            .Setup(r => r.ByIdAsync(expectedTenantId))
            .ReturnsAsync(expectedTenant);
        
        var job = new MetaImportJob<FakeTenant, IRepository<FakeTenant>>(
            ServiceProvider,
            JobMetaRepositoryMock.Object,
            TenantMetaRepositoryMock.Object,
            TenantRightsCheckerMock.Object,
            JobsFileStorageAdapterMock.Object);
        
        var jobDataMapNoIdentifier = new JobDataMap
        {
            { "tenantId", expectedTenantId },
            { "jobId", expectedJobId },
            { "userId", expectedUserId },
            { "claims", JsonConvert.SerializeObject(expectedClaims) },
            { "file", expectedTemporaryId }
        };

        JobExecutionContextMock
            .Setup(c => c.MergedJobDataMap)
            .Returns(jobDataMapNoIdentifier);
        
        // Act
        Assert.ThrowsAsync<JobExecutionException>(async () => await job.Execute(JobExecutionContextMock.Object));
        
        var jobDataMapNoFilename = new JobDataMap
        {
            { "tenantId", expectedTenantId },
            { "jobId", expectedJobId },
            { "userId", expectedUserId },
            { "identifier", ExpectedFunctionIdentifier },
            { "claims", JsonConvert.SerializeObject(expectedClaims) },
        };
        
        JobExecutionContextMock
            .Setup(c => c.MergedJobDataMap)
            .Returns(jobDataMapNoFilename);
        
        // Act
        Assert.ThrowsAsync<JobExecutionException>(async () => await job.Execute(JobExecutionContextMock.Object), $"Tenant {expectedTenantId} unknown");

        
        // Assert
        JobMetaRepositoryMock.Verify(
            r => r.UpdateJobAsync(expectedTenantId, expectedUserId, expectedJobId, JobStates.InProgress, string.Empty),
            Times.Never);
        
        JobsFileStorageAdapterMock.Verify(s => s.TemporaryFileByIdAsync(expectedTenantId, expectedTemporaryId), 
            Times.Never);
        JobsFileStorageAdapterMock.Verify(s => s.RemoveTemporaryFileByIdBehalfOfUserAsync(expectedTenantId, expectedUserId, expectedTemporaryId), 
            Times.Never);
        
        RepositoryMock.Verify(r => r.ImportAsync(
            expectedUserId,
            ExpectedFunctionIdentifier,
            expectedClaims,
            It.IsAny<Stream>(),
            It.IsAny<Func<FakeTenant, Task<bool>>>()), 
            Times.Never);
    }
}