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
public class PickvalueEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<IPickvalueMetaRepository> _pickvalueRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _pickvalueRepositoryMock = new Mock<IPickvalueMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPickvalueMetaRepository))).Returns(_pickvalueRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    // ---- HandlePickvalueAvailabilityAsync ----

    [Test]
    public async Task HandlePickvalueAvailabilityAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueAvailabilityAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandlePickvalueAvailabilityAsync_Success_ReturnsAvailabilityList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var availabilityData = new List<PickvalueAvailability>
        {
            new() { Entity = "Order", Field = "Status" },
            new() { Entity = "Customer", Field = "Type" }
        };
        var availabilitySummaries = new[]
        {
            new PickvalueAvailabilitySummary { Entity = "Order", Field = "Status" },
            new PickvalueAvailabilitySummary { Entity = "Customer", Field = "Type" }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pickvalueRepositoryMock.Setup(r => r.GetPickvalueAvailabilityAsync(tenantId)).ReturnsAsync(availabilityData);
        _mapperMock.Setup(m => m.Map<PickvalueAvailabilitySummary[]>(availabilityData)).Returns(availabilitySummaries);

        var result = await PickvalueTools.HandlePickvalueAvailabilityAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var availabilities = jsonElement.GetProperty("availabilities");
        Assert.That(availabilities.GetArrayLength(), Is.EqualTo(2));
        Assert.That(availabilities[0].GetProperty("entity").GetString(), Is.EqualTo("Order"));
        Assert.That(availabilities[0].GetProperty("field").GetString(), Is.EqualTo("Status"));
    }

    // ---- HandlePickvalueSelectListAsync ----

    [Test]
    public async Task HandlePickvalueSelectListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandlePickvalueSelectListAsync_MissingEntity_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectListAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "field", "Status" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'entity' argument"));
    }

    [Test]
    public async Task HandlePickvalueSelectListAsync_MissingField_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectListAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "entity", "Order" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'field' argument"));
    }

    [Test]
    public async Task HandlePickvalueSelectListAsync_Success_ReturnsPickvalueList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var pickvalueData = new List<PickvalueSelectEntry>
        {
            new() { Id = Guid.NewGuid(), Name = "Open", Value = 1 },
            new() { Id = Guid.NewGuid(), Name = "Closed", Value = 2 }
        };
        var pickvalueSummaries = new[]
        {
            new PickvalueSelectEntrySummary { Id = pickvalueData[0].Id, Name = "Open", Value = 1 },
            new PickvalueSelectEntrySummary { Id = pickvalueData[1].Id, Name = "Closed", Value = 2 }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pickvalueRepositoryMock.Setup(r => r.SelectListForEntityFieldAsync(tenantId, "Order", "Status")).ReturnsAsync(pickvalueData);
        _mapperMock.Setup(m => m.Map<PickvalueSelectEntrySummary[]>(pickvalueData)).Returns(pickvalueSummaries);

        var result = await PickvalueTools.HandlePickvalueSelectListAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "field", "Status" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var pickvalues = jsonElement.GetProperty("pickvalues");
        Assert.That(pickvalues.GetArrayLength(), Is.EqualTo(2));
        Assert.That(pickvalues[0].GetProperty("name").GetString(), Is.EqualTo("Open"));
        Assert.That(pickvalues[1].GetProperty("value").GetInt32(), Is.EqualTo(2));
    }

    // ---- HandlePickvalueSelectByValueAsync ----

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_MissingEntity_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "field", "Status" }, { "value", "1" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'entity' argument"));
    }

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_MissingField_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "entity", "Order" }, { "value", "1" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'field' argument"));
    }

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_MissingValue_ReturnsErrorMessage()
    {
        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "entity", "Order" }, { "field", "Status" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'value' argument"));
    }

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_NotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pickvalueRepositoryMock.Setup(r => r.SelectByValueAsync(tenantId, "Order", "Status", 99)).ReturnsAsync((PickvalueSelectEntry?)null);

        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "field", "Status" }, { "value", "99" } });

        Assert.That(result.Text, Is.EqualTo("Pickvalue not found for entity 'Order', field 'Status', value 99"));
    }

    [Test]
    public async Task HandlePickvalueSelectByValueAsync_Success_ReturnsPickvalueEntry()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var pickvalueData = new PickvalueSelectEntry { Id = id, Name = "Open", Value = 1 };
        var pickvalueSummary = new PickvalueSelectEntrySummary { Id = id, Name = "Open", Value = 1 };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pickvalueRepositoryMock.Setup(r => r.SelectByValueAsync(tenantId, "Order", "Status", 1)).ReturnsAsync(pickvalueData);
        _mapperMock.Setup(m => m.Map<PickvalueSelectEntrySummary>(pickvalueData)).Returns(pickvalueSummary);

        var result = await PickvalueTools.HandlePickvalueSelectByValueAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "field", "Status" }, { "value", "1" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(id.ToString()));
        Assert.That(jsonElement.GetProperty("name").GetString(), Is.EqualTo("Open"));
        Assert.That(jsonElement.GetProperty("value").GetInt32(), Is.EqualTo(1));
    }

    // ---- Registration ----

    [Test]
    public void RegisterBallwarePickvalueTools_RegistersExpectedTools()
    {
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterStaticTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        registryMock.Object.RegisterBallwarePickvalueTools();

        Assert.That(registeredTools, Has.Count.EqualTo(3));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.pickvalue.availability"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.pickvalue.selectlistforentityandfield"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.pickvalue.selectbyvalueforentityandfield"), Is.True);
    }
}
