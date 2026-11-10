using System.Security.Claims;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Meta.Mcp.Endpoints;
using Ballware.Meta.Mcp.Public;
using Ballware.Shared.Authorization;
using Ballware.Shared.Mcp;
using MapsterMapper;
using Moq;

namespace Ballware.Meta.Mcp.Tests.Endpoints;

[TestFixture]
public class StatisticEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<IStatisticMetaRepository> _statisticRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _statisticRepositoryMock = new Mock<IStatisticMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IStatisticMetaRepository))).Returns(_statisticRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    // ---- HandleStatisticListAsync ----

    [Test]
    public async Task HandleStatisticListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await StatisticTools.HandleStatisticListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleStatisticListAsync_Success_ReturnsStatisticList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var statisticData = new List<StatisticSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Identifier = "stat1", Name = "Statistic 1" },
            new() { Id = Guid.NewGuid(), Identifier = "stat2", Name = "Statistic 2" }
        };
        var statisticSummaries = new[]
        {
            new StatisticSummary { Id = statisticData[0].Id, Identifier = "stat1", Name = "Statistic 1" },
            new StatisticSummary { Id = statisticData[1].Id, Identifier = "stat2", Name = "Statistic 2" }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _statisticRepositoryMock.Setup(r => r.SelectListForTenantAsync(tenantId)).ReturnsAsync(statisticData);
        _mapperMock.Setup(m => m.Map<StatisticSummary[]>(statisticData)).Returns(statisticSummaries);

        var result = await StatisticTools.HandleStatisticListAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var statistics = jsonElement.GetProperty("statistics");
        Assert.That(statistics.GetArrayLength(), Is.EqualTo(2));
        Assert.That(statistics[0].GetProperty("identifier").GetString(), Is.EqualTo("stat1"));
        Assert.That(statistics[1].GetProperty("name").GetString(), Is.EqualTo("Statistic 2"));
    }

    // ---- HandleStatisticByIdAsync ----

    [Test]
    public async Task HandleStatisticByIdAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await StatisticTools.HandleStatisticByIdAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleStatisticByIdAsync_MissingId_ReturnsErrorMessage()
    {
        var result = await StatisticTools.HandleStatisticByIdAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'id' argument"));
    }

    [Test]
    public async Task HandleStatisticByIdAsync_NotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _statisticRepositoryMock.Setup(r => r.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync((StatisticSelectListEntry?)null);

        var result = await StatisticTools.HandleStatisticByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.Text, Is.EqualTo($"Statistic not found with id {id}"));
    }

    [Test]
    public async Task HandleStatisticByIdAsync_Success_ReturnsStatisticSummary()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var statisticData = new StatisticSelectListEntry { Id = id, Identifier = "stat1", Name = "Statistic 1" };
        var statisticSummary = new StatisticSummary { Id = id, Identifier = "stat1", Name = "Statistic 1" };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _statisticRepositoryMock.Setup(r => r.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync(statisticData);
        _mapperMock.Setup(m => m.Map<StatisticSummary>(statisticData)).Returns(statisticSummary);

        var result = await StatisticTools.HandleStatisticByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(id.ToString()));
        Assert.That(jsonElement.GetProperty("identifier").GetString(), Is.EqualTo("stat1"));
    }

    // ---- HandleStatisticMetadataByIdentifierAsync ----

    [Test]
    public async Task HandleStatisticMetadataByIdentifierAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await StatisticTools.HandleStatisticMetadataByIdentifierAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleStatisticMetadataByIdentifierAsync_MissingIdentifier_ReturnsErrorMessage()
    {
        var result = await StatisticTools.HandleStatisticMetadataByIdentifierAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'identifier' argument"));
    }

    [Test]
    public async Task HandleStatisticMetadataByIdentifierAsync_NotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _statisticRepositoryMock.Setup(r => r.MetadataByIdentifierAsync(tenantId, "stat1")).ReturnsAsync((Statistic?)null);

        var result = await StatisticTools.HandleStatisticMetadataByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", "stat1" } });

        Assert.That(result.Text, Is.EqualTo("Statistic not found with identifier 'stat1'"));
    }

    [Test]
    public async Task HandleStatisticMetadataByIdentifierAsync_Success_ReturnsStatisticMetadata()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var statisticData = new Statistic { Id = id, Identifier = "stat1", Name = "Statistic 1", Entity = "Order", Meta = false };
        var statisticMetadata = new StatisticMetadata { Id = id, Identifier = "stat1", Name = "Statistic 1", Entity = "Order", Meta = false };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _statisticRepositoryMock.Setup(r => r.MetadataByIdentifierAsync(tenantId, "stat1")).ReturnsAsync(statisticData);
        _mapperMock.Setup(m => m.Map<StatisticMetadata>(statisticData)).Returns(statisticMetadata);

        var result = await StatisticTools.HandleStatisticMetadataByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", "stat1" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(id.ToString()));
        Assert.That(jsonElement.GetProperty("identifier").GetString(), Is.EqualTo("stat1"));
        Assert.That(jsonElement.GetProperty("entity").GetString(), Is.EqualTo("Order"));
        Assert.That(jsonElement.GetProperty("meta").GetBoolean(), Is.False);
    }

    // ---- Registration ----

    [Test]
    public void RegisterBallwareStatisticTools_RegistersExpectedTools()
    {
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterStaticTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        registryMock.Object.RegisterBallwareStatisticTools();

        Assert.That(registeredTools, Has.Count.EqualTo(3));
        Assert.That(registeredTools.Any(t => t.Name == "ballware_meta_statistic_list"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware_meta_statistic_by_id"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware_meta_statistic_metadata_by_identifier"), Is.True);
    }
}
