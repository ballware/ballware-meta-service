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
public class ProcessingStateEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<IProcessingStateMetaRepository> _processingStateRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _processingStateRepositoryMock = new Mock<IProcessingStateMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IProcessingStateMetaRepository))).Returns(_processingStateRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    // ---- HandleProcessingStateListAsync ----

    [Test]
    public async Task HandleProcessingStateListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleProcessingStateListAsync_Success_ReturnsProcessingStateList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var stateData = new List<ProcessingStateSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false },
            new() { Id = Guid.NewGuid(), Name = "Closed", State = 2, Locked = true, Finished = true, ReasonRequired = false }
        };
        var stateSummaries = new[]
        {
            new ProcessingStateSummary { Id = stateData[0].Id, Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false },
            new ProcessingStateSummary { Id = stateData[1].Id, Name = "Closed", State = 2, Locked = true, Finished = true, ReasonRequired = false }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectListForTenantAsync(tenantId)).ReturnsAsync(stateData);
        _mapperMock.Setup(m => m.Map<ProcessingStateSummary[]>(stateData)).Returns(stateSummaries);

        var result = await ProcessingStateTools.HandleProcessingStateListAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var states = jsonElement.GetProperty("processingStates");
        Assert.That(states.GetArrayLength(), Is.EqualTo(2));
        Assert.That(states[0].GetProperty("name").GetString(), Is.EqualTo("Open"));
        Assert.That(states[1].GetProperty("finished").GetBoolean(), Is.True);
    }

    // ---- HandleProcessingStateByIdAsync ----

    [Test]
    public async Task HandleProcessingStateByIdAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateByIdAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleProcessingStateByIdAsync_MissingId_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateByIdAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'id' argument"));
    }

    [Test]
    public async Task HandleProcessingStateByIdAsync_NotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync((ProcessingStateSelectListEntry?)null);

        var result = await ProcessingStateTools.HandleProcessingStateByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.Text, Is.EqualTo($"Processing state not found with id {id}"));
    }

    [Test]
    public async Task HandleProcessingStateByIdAsync_Success_ReturnsProcessingStateSummary()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var stateData = new ProcessingStateSelectListEntry { Id = id, Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false };
        var stateSummary = new ProcessingStateSummary { Id = id, Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectByIdForTenantAsync(tenantId, id)).ReturnsAsync(stateData);
        _mapperMock.Setup(m => m.Map<ProcessingStateSummary>(stateData)).Returns(stateSummary);

        var result = await ProcessingStateTools.HandleProcessingStateByIdAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "id", id.ToString() } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(id.ToString()));
        Assert.That(jsonElement.GetProperty("state").GetInt32(), Is.EqualTo(1));
    }

    // ---- HandleProcessingStateListForEntityAsync ----

    [Test]
    public async Task HandleProcessingStateListForEntityAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateListForEntityAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleProcessingStateListForEntityAsync_MissingEntity_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateListForEntityAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'entity' argument"));
    }

    [Test]
    public async Task HandleProcessingStateListForEntityAsync_Success_ReturnsProcessingStateList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var stateData = new List<ProcessingStateSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false }
        };
        var stateSummaries = new[]
        {
            new ProcessingStateSummary { Id = stateData[0].Id, Name = "Open", State = 1 }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectListForEntityAsync(tenantId, "Order")).ReturnsAsync(stateData);
        _mapperMock.Setup(m => m.Map<ProcessingStateSummary[]>(stateData)).Returns(stateSummaries);

        var result = await ProcessingStateTools.HandleProcessingStateListForEntityAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var states = jsonElement.GetProperty("processingStates");
        Assert.That(states.GetArrayLength(), Is.EqualTo(1));
        Assert.That(states[0].GetProperty("name").GetString(), Is.EqualTo("Open"));
    }

    // ---- HandleProcessingStateByStateForEntityAsync ----

    [Test]
    public async Task HandleProcessingStateByStateForEntityAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateByStateForEntityAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleProcessingStateByStateForEntityAsync_MissingEntity_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateByStateForEntityAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "state", "1" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'entity' argument"));
    }

    [Test]
    public async Task HandleProcessingStateByStateForEntityAsync_MissingState_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateByStateForEntityAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "entity", "Order" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'state' argument"));
    }

    [Test]
    public async Task HandleProcessingStateByStateForEntityAsync_NotFound_ReturnsErrorMessage()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectByStateAsync(tenantId, "Order", 99)).ReturnsAsync((ProcessingStateSelectListEntry?)null);

        var result = await ProcessingStateTools.HandleProcessingStateByStateForEntityAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "state", "99" } });

        Assert.That(result.Text, Is.EqualTo("Processing state not found for entity 'Order', state 99"));
    }

    [Test]
    public async Task HandleProcessingStateByStateForEntityAsync_Success_ReturnsProcessingStateSummary()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var stateData = new ProcessingStateSelectListEntry { Id = id, Name = "Open", State = 1, Locked = false, Finished = false, ReasonRequired = false };
        var stateSummary = new ProcessingStateSummary { Id = id, Name = "Open", State = 1 };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectByStateAsync(tenantId, "Order", 1)).ReturnsAsync(stateData);
        _mapperMock.Setup(m => m.Map<ProcessingStateSummary>(stateData)).Returns(stateSummary);

        var result = await ProcessingStateTools.HandleProcessingStateByStateForEntityAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "state", "1" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("name").GetString(), Is.EqualTo("Open"));
        Assert.That(jsonElement.GetProperty("state").GetInt32(), Is.EqualTo(1));
    }

    // ---- HandleProcessingStateSuccessorsForEntityAndStateAsync ----

    [Test]
    public async Task HandleProcessingStateSuccessorsForEntityAndStateAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateSuccessorsForEntityAndStateAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleProcessingStateSuccessorsForEntityAndStateAsync_MissingEntity_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateSuccessorsForEntityAndStateAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "state", "1" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'entity' argument"));
    }

    [Test]
    public async Task HandleProcessingStateSuccessorsForEntityAndStateAsync_MissingState_ReturnsErrorMessage()
    {
        var result = await ProcessingStateTools.HandleProcessingStateSuccessorsForEntityAndStateAsync(_serviceProviderMock.Object, new ClaimsPrincipal(),
            new Dictionary<string, object?> { { "entity", "Order" } });

        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'state' argument"));
    }

    [Test]
    public async Task HandleProcessingStateSuccessorsForEntityAndStateAsync_Success_ReturnsProcessingStateList()
    {
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var stateData = new List<ProcessingStateSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Name = "In Progress", State = 2 },
            new() { Id = Guid.NewGuid(), Name = "Closed", State = 3 }
        };
        var stateSummaries = new[]
        {
            new ProcessingStateSummary { Id = stateData[0].Id, Name = "In Progress", State = 2 },
            new ProcessingStateSummary { Id = stateData[1].Id, Name = "Closed", State = 3 }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _processingStateRepositoryMock.Setup(r => r.SelectListPossibleSuccessorsForEntityAsync(tenantId, "Order", 1)).ReturnsAsync(stateData);
        _mapperMock.Setup(m => m.Map<ProcessingStateSummary[]>(stateData)).Returns(stateSummaries);

        var result = await ProcessingStateTools.HandleProcessingStateSuccessorsForEntityAndStateAsync(_serviceProviderMock.Object, principal,
            new Dictionary<string, object?> { { "entity", "Order" }, { "state", "1" } });

        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var states = jsonElement.GetProperty("processingStates");
        Assert.That(states.GetArrayLength(), Is.EqualTo(2));
        Assert.That(states[0].GetProperty("name").GetString(), Is.EqualTo("In Progress"));
        Assert.That(states[1].GetProperty("state").GetInt32(), Is.EqualTo(3));
    }

    // ---- Registration ----

    [Test]
    public void RegisterBallwareProcessingStateTools_RegistersExpectedTools()
    {
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterStaticTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        registryMock.Object.RegisterBallwareProcessingStateTools();

        Assert.That(registeredTools, Has.Count.EqualTo(5));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.processingstate.list"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.processingstate.by.id"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.processingstate.listforentity"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.processingstate.bystateforentity"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.processingstate.successorsforentityandstate"), Is.True);
    }
}
