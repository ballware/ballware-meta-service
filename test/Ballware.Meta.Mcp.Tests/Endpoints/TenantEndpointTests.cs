using System.Security.Claims;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Mcp.Endpoints;
using Ballware.Meta.Mcp.Public;
using Ballware.Shared.Authorization;
using Ballware.Shared.Mcp;
using MapsterMapper;
using Moq;

namespace Ballware.Meta.Mcp.Tests.Endpoints;

[TestFixture]
public class TenantEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<ITenantMetaRepository> _tenantRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ITenantRightsChecker> _tenantRightsCheckerMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _tenantRepositoryMock = new Mock<ITenantMetaRepository>();
        _mapperMock = new Mock<IMapper>();
        _tenantRightsCheckerMock = new Mock<ITenantRightsChecker>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ITenantMetaRepository))).Returns(_tenantRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ITenantRightsChecker))).Returns(_tenantRightsCheckerMock.Object);
    }

    [Test]
    public async Task HandleTenantCurrentSummaryAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await TenantTools.HandleTenantCurrentSummaryAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleTenantCurrentSummaryAsync_TenantNotFound_ReturnsErrorMessage()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _tenantRepositoryMock.Setup(tr => tr.ByIdAsync(tenantId)).ReturnsAsync((Tenant?)null);

        // Act
        var result = await TenantTools.HandleTenantCurrentSummaryAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo($"No tenant found for id {tenantId}"));
    }

    [Test]
    public async Task HandleTenantCurrentSummaryAsync_Success_ReturnsTenantSummary()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var tenantData = new Tenant { Id = tenantId, Name = "Test Tenant" };
        var tenantSummary = new TenantSummary { Id = tenantId, Name = "Test Tenant" };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _tenantRepositoryMock.Setup(tr => tr.ByIdAsync(tenantId)).ReturnsAsync(tenantData);
        _mapperMock.Setup(m => m.Map<TenantSummary>(tenantData)).Returns(tenantSummary);

        // Act
        var result = await TenantTools.HandleTenantCurrentSummaryAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(tenantId.ToString()));
        Assert.That(jsonElement.GetProperty("name").GetString(), Is.EqualTo("Test Tenant"));
    }

    [Test]
    public async Task HandleTenantCurrentNavigationAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await TenantTools.HandleTenantCurrentNavigationAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandleTenantCurrentNavigationAsync_TenantNotFound_ReturnsErrorMessage()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _tenantRepositoryMock.Setup(tr => tr.ByIdAsync(tenantId)).ReturnsAsync((Tenant?)null);

        // Act
        var result = await TenantTools.HandleTenantCurrentNavigationAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo($"No tenant found for id {tenantId}"));
    }

    [Test]
    public async Task HandleTenantCurrentNavigationAsync_Success_ReturnsFilteredNavigation()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var claims = new Dictionary<string, object> { { "role", "admin" } };
        var tenantData = new Tenant { Id = tenantId, Name = "Test Tenant" };
        var navigationLayout = new NavigationLayout
        {
            Title = "Test Title",
            Items = new List<NavigationLayoutItem>
            {
                new NavigationLayoutItem
                {
                    Type = NavigationLayoutItemType.Page,
                    Options = new NavigationLayoutItemOptions { Page = "page1", Caption = "Page 1" }
                },
                new NavigationLayoutItem
                {
                    Type = NavigationLayoutItemType.Page,
                    Options = new NavigationLayoutItemOptions { Page = "page2", Caption = "Page 2" }
                },
                new NavigationLayoutItem
                {
                    Type = NavigationLayoutItemType.Group,
                    Options = new NavigationLayoutItemOptions { Caption = "Group 1" },
                    Items = new List<NavigationLayoutItem>
                    {
                        new NavigationLayoutItem
                        {
                            Type = NavigationLayoutItemType.Page,
                            Options = new NavigationLayoutItemOptions { Page = "page3", Caption = "Page 3" }
                        }
                    }
                }
            }
        };
        var tenantNavigation = new TenantNavigation { Id = tenantId, Layout = navigationLayout };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _principalUtilsMock.Setup(pu => pu.GetUserClaims(principal)).Returns(claims);
        _tenantRepositoryMock.Setup(tr => tr.ByIdAsync(tenantId)).ReturnsAsync(tenantData);
        _mapperMock.Setup(m => m.Map<TenantNavigation>(tenantData)).Returns(tenantNavigation);

        // page1 and page3 are allowed, page2 is not
        _tenantRightsCheckerMock.Setup(trc => trc.HasRightAsync(tenantData, "generic", "page", claims, "page1")).ReturnsAsync(true);
        _tenantRightsCheckerMock.Setup(trc => trc.HasRightAsync(tenantData, "generic", "page", claims, "page2")).ReturnsAsync(false);
        _tenantRightsCheckerMock.Setup(trc => trc.HasRightAsync(tenantData, "generic", "page", claims, "page3")).ReturnsAsync(true);

        // Act
        var result = await TenantTools.HandleTenantCurrentNavigationAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var items = jsonElement.GetProperty("layout").GetProperty("items");
        
        Assert.That(items.GetArrayLength(), Is.EqualTo(2)); // page1 and Group 1 (which contains page3)
        
        Assert.That(items[0].GetProperty("options").GetProperty("page").GetString(), Is.EqualTo("page1"));
        Assert.That(items[1].GetProperty("options").GetProperty("caption").GetString(), Is.EqualTo("Group 1"));
        
        var groupItems = items[1].GetProperty("items");
        Assert.That(groupItems.GetArrayLength(), Is.EqualTo(1));
        Assert.That(groupItems[0].GetProperty("options").GetProperty("page").GetString(), Is.EqualTo("page3"));
    }

    [Test]
    public void RegisterBallwareTenantTools_RegistersExpectedTools()
    {
        // Arrange
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterStaticTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        // Act
        registryMock.Object.RegisterBallwareTenantTools();

        // Assert
        Assert.That(registeredTools, Has.Count.EqualTo(2));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.tenant.current.summary"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.tenant.current.navigation"), Is.True);
    }
}