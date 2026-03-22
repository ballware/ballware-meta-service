using System.Security.Claims;
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
public class LookupEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<ILookupMetaRepository> _lookupRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _lookupRepositoryMock = new Mock<ILookupMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILookupMetaRepository))).Returns(_lookupRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    [Test]
    public async Task HandleLookupListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await LookupTools.HandleLookupListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleLookupListAsync_Success_ReturnsLookupList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var lookupData = new List<LookupSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Identifier = "lookup1", Name = "Lookup 1", HasParam = false },
            new() { Id = Guid.NewGuid(), Identifier = "lookup2", Name = "Lookup 2", HasParam = true }
        };
        var lookupSummaries = new[]
        {
            new LookupSummary { Id = lookupData[0].Id, Identifier = "lookup1", Name = "Lookup 1", HasParam = false },
            new LookupSummary { Id = lookupData[1].Id, Identifier = "lookup2", Name = "Lookup 2", HasParam = true }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _lookupRepositoryMock.Setup(tr => tr.SelectListForTenantAsync(tenantId)).ReturnsAsync(lookupData);
        _mapperMock.Setup(m => m.Map<LookupSummary[]>(lookupData)).Returns(lookupSummaries);

        var result = await LookupTools.HandleLookupListAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var lookups = jsonElement.GetProperty("lookups");
        Assert.That(lookups.GetArrayLength(), Is.EqualTo(2));
        Assert.That(lookups[0].GetProperty("identifier").GetString(), Is.EqualTo("lookup1"));
        Assert.That(lookups[1].GetProperty("hasParam").GetBoolean(), Is.True);
    }

    [Test]
    public async Task HandleLookupByIdAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await LookupTools.HandleLookupByIdAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleLookupByIdAsync_MissingId_ReturnsErrorMessage()
    {
        var result = await LookupTools.HandleLookupByIdAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'id' argument"));
    }

    [Test]
    public async Task HandleLookupByIdAsync_LookupNotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _lookupRepositoryMock.Setup(tr => tr.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync((LookupSelectListEntry?)null);

        var result = await LookupTools.HandleLookupByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.Text, Is.EqualTo($"Lookup not found with id {id}"));
    }

    [Test]
    public async Task HandleLookupByIdAsync_Success_ReturnsLookupSummary()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var lookupData = new LookupSelectListEntry { Id = id, Identifier = "lookup1", Name = "Lookup 1", HasParam = true };
        var lookupSummary = new LookupSummary { Id = id, Identifier = "lookup1", Name = "Lookup 1", HasParam = true };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _lookupRepositoryMock.Setup(tr => tr.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync(lookupData);
        _mapperMock.Setup(m => m.Map<LookupSummary>(lookupData)).Returns(lookupSummary);

        var result = await LookupTools.HandleLookupByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(id.ToString()));
        Assert.That(jsonElement.GetProperty("identifier").GetString(), Is.EqualTo("lookup1"));
        Assert.That(jsonElement.GetProperty("hasParam").GetBoolean(), Is.True);
    }

    [Test]
    public void RegisterBallwareLookupTools_RegistersExpectedTools()
    {
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterStaticTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        registryMock.Object.RegisterBallwareLookupTools();

        Assert.That(registeredTools, Has.Count.EqualTo(2));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.lookup.list"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.lookup.by.id"), Is.True);
    }
}
