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
public class PageEndpointTests
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IPrincipalUtils> _principalUtilsMock;
    private Mock<IPageMetaRepository> _pageRepositoryMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void SetUp()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _principalUtilsMock = new Mock<IPrincipalUtils>();
        _pageRepositoryMock = new Mock<IPageMetaRepository>();
        _mapperMock = new Mock<IMapper>();

        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPrincipalUtils))).Returns(_principalUtilsMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPageMetaRepository))).Returns(_pageRepositoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IMapper))).Returns(_mapperMock.Object);
    }

    [Test]
    public async Task HandlePageListAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await PageTools.HandlePageListAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandlePageListAsync_Success_ReturnsPageList()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var pageData = new List<PageSelectListEntry>
        {
            new() { Id = Guid.NewGuid(), Identifier = "page1", Name = "Page 1" },
            new() { Id = Guid.NewGuid(), Identifier = "page2", Name = "Page 2" }
        };
        var pageSummaries = new[]
        {
            new PageSummary { Id = pageData[0].Id, Identifier = "page1", Name = "Page 1" },
            new PageSummary { Id = pageData[1].Id, Identifier = "page2", Name = "Page 2" }
        };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pageRepositoryMock.Setup(tr => tr.SelectListForTenantAsync(tenantId)).ReturnsAsync(pageData);
        _mapperMock.Setup(m => m.Map<PageSummary[]>(pageData)).Returns(pageSummaries);

        // Act
        var result = await PageTools.HandlePageListAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        var pages = jsonElement.GetProperty("pages");
        Assert.That(pages.GetArrayLength(), Is.EqualTo(2));
        Assert.That(pages[0].GetProperty("identifier").GetString(), Is.EqualTo("page1"));
        Assert.That(pages[1].GetProperty("identifier").GetString(), Is.EqualTo("page2"));
    }

    [Test]
    public async Task HandlePageByIdentifierAsync_UserNotAuthenticated_ReturnsErrorMessage()
    {
        // Act
        var result = await PageTools.HandlePageByIdentifierAsync(_serviceProviderMock.Object, null, new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("User is not authenticated"));
    }

    [Test]
    public async Task HandlePageByIdentifierAsync_MissingIdentifier_ReturnsErrorMessage()
    {
        // Act
        var result = await PageTools.HandlePageByIdentifierAsync(_serviceProviderMock.Object, new ClaimsPrincipal(), new Dictionary<string, object?>());

        // Assert
        Assert.That(result.Text, Is.EqualTo("Missing or invalid 'identifier' argument"));
    }

    [Test]
    public async Task HandlePageByIdentifierAsync_PageNotFound_ReturnsErrorMessage()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var identifier = "unknown";
        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pageRepositoryMock.Setup(tr => tr.ByIdentifierAsync(tenantId, identifier)).ReturnsAsync((Data.Public.Page?)null);

        // Act
        var result = await PageTools.HandlePageByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", identifier } });

        // Assert
        Assert.That(result.Text, Is.EqualTo($"Page not found with identifier {identifier}"));
    }

    [Test]
    public async Task HandlePageByIdentifierAsync_Success_ReturnsPageSummary()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var tenantId = Guid.NewGuid();
        var identifier = "page1";
        var pageData = new Data.Public.Page { Id = Guid.NewGuid(), Identifier = identifier, Name = "Page 1" };
        var pageSummary = new PageSummary { Id = pageData.Id, Identifier = identifier, Name = "Page 1" };

        _principalUtilsMock.Setup(pu => pu.GetUserTenandId(principal)).Returns(tenantId);
        _pageRepositoryMock.Setup(tr => tr.ByIdentifierAsync(tenantId, identifier)).ReturnsAsync(pageData);
        _mapperMock.Setup(m => m.Map<PageSummary>(pageData)).Returns(pageSummary);

        // Act
        var result = await PageTools.HandlePageByIdentifierAsync(_serviceProviderMock.Object, principal, new Dictionary<string, object?> { { "identifier", identifier } });

        // Assert
        Assert.That(result.StructuredContent, Is.Not.Null);
        var jsonElement = result.StructuredContent!.Value;
        Assert.That(jsonElement.GetProperty("id").GetString(), Is.EqualTo(pageData.Id.ToString()));
        Assert.That(jsonElement.GetProperty("identifier").GetString(), Is.EqualTo(identifier));
    }

    [Test]
    public void RegisterBallwarePageTools_RegistersExpectedTools()
    {
        // Arrange
        var registryMock = new Mock<IToolRegistry>();
        var registeredTools = new List<Tool>();
        registryMock.Setup(r => r.RegisterTool(It.IsAny<Tool>())).Callback<Tool>(t => registeredTools.Add(t));

        // Act
        registryMock.Object.RegisterBallwarePageTools();

        // Assert
        Assert.That(registeredTools, Has.Count.EqualTo(2));
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.page.list"), Is.True);
        Assert.That(registeredTools.Any(t => t.Name == "ballware.meta.page.by.identifier"), Is.True);
    }
}
