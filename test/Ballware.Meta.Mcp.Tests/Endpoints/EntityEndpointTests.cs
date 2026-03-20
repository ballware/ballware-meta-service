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
public class EntityEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<IEntityMetaRepository> _entityRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _entityRepositoryMock = new Mock<IEntityMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEntityMetaRepository))).Returns(_entityRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    [Test]
    public async Task HandleEntityListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await EntityTools.HandleEntityListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleEntityListAsync_Success_ReturnsEntityList()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var entityData = new List<EntitySelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Application = "test", Entity = "entity1", Name = "Entity 1" },
            new() { Id = Guid.NewGuid(), Application = "test", Entity = "entity2", Name = "Entity 2" }
        };
        var entitySummaries = new[]
        {
            new EntitySummary { Id = entityData[0].Id, Entity = "entity1", Name = "Entity 1" },
            new EntitySummary { Id = entityData[1].Id, Entity = "entity2", Name = "Entity 2" }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _entityRepositoryMock.Setup(tr => tr.SelectListForTenantAsync(tenantId)).ReturnsAsync(entityData);
        _mapperMock.Setup(m => m.Map<EntitySummary[]>(entityData)).Returns(entitySummaries);

        // Act
        var result = await EntityTools.HandleEntityListAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var entities = jsonElement.GetProperty("entities");
        Assert.That(entities.GetArrayLength(), Is.EqualTo(2));
        Assert.That(entities[0].GetProperty("entity").GetString(), Is.EqualTo("entity1"));
        Assert.That(entities[1].GetProperty("entity").GetString(), Is.EqualTo("entity2"));
    }

    [Test]
    public async Task HandleEntityByIdentifierAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await EntityTools.HandleEntityByIdentifierAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleEntityByIdentifierAsync_MissingIdentifier_ReturnsErrorMessage()
    {
        // Act
        var result = await EntityTools.HandleEntityByIdentifierAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'identifier' argument"));
    }

    [Test]
    public async Task HandleEntityByIdentifierAsync_EntityNotFound_ReturnsErrorMessage()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var identifier = "unknown";
        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _entityRepositoryMock.Setup(tr => tr.SelectByIdentifierForTenantAsync(tenantId, identifier)).ReturnsAsync((EntitySelectListEntry?)null);

        // Act
        var result = await EntityTools.HandleEntityByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", identifier } });

        // Assert
        Assert.That(result.Text, Is.EqualTo($"Entity not found with identifier {identifier}"));
    }

    [Test]
    public async Task HandleEntityByIdentifierAsync_Success_ReturnsEntitySummary()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var identifier = "entity1";
        var entityData = new EntitySelectListEntry { Id = Guid.NewGuid(), Application = "test", Entity = identifier, Name = "Entity 1" };
        var entitySummary = new EntitySummary { Id = entityData.Id, Entity = identifier, Name = "Entity 1" };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _entityRepositoryMock.Setup(tr => tr.SelectByIdentifierForTenantAsync(tenantId, identifier)).ReturnsAsync(entityData);
        _mapperMock.Setup(m => m.Map<EntitySummary>(entityData)).Returns(entitySummary);

        // Act
        var result = await EntityTools.HandleEntityByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", identifier } });

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(entityData.Id.ToString()));
        Assert.That(jsonElement.GetProperty("entity").GetString(), Is.EqualTo(identifier));
    }

    [Test]
    public void RegisterBallwareEntityTools_RegistersExpectedTools()
    {
        // Arrange
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        // Act
        registryMock.Object.RegisterBallwareEntityTools();

        // Assert
        Assert.That(registeredTools, Has.Count.EqualTo(2));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.entity.list"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.entity.by.identifier"), Is.True);
    }
}
