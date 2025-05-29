using System.Text.Json;
using Ballware.Meta.Authorization.Jint.Internal;
using Ballware.Meta.Data.Public;
using Moq;

namespace Ballware.Meta.Authorization.Jint.Tests;

[TestFixture]
public class JavascriptEntityRightsCheckerTest
{
    [Test]
    public void HasRightAsync_ShouldReturnTrue_WhenRightsCheckScriptIsEmpty()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        
        // Arrange
        var customScripts = new EntityCustomScripts()
        {
            ExtendedRightsCheck = null
        };
        
        var entitymetadataMock = new Mock<EntityMetadata>();
        entitymetadataMock
            .Setup(m => m.CustomScripts)
            .Returns(JsonSerializer.Serialize(customScripts));
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.HasRightAsync(
            expectedTenantId,
            entitymetadataMock.Object,
            new Dictionary<string, object> { { "right", new string[] { "add", "custom" } } },
            "edit",
            new { Id = Guid.NewGuid() },
            true
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void HasRightAsync_ShouldReturnTrue_WhenTenantResultIsTrue()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        
        // Arrange
        var customScripts = new EntityCustomScripts()
        {
            ExtendedRightsCheck = "return result;"
        };
        
        var entitymetadataMock = new Mock<EntityMetadata>();
        entitymetadataMock
            .Setup(m => m.CustomScripts)
            .Returns(JsonSerializer.Serialize(customScripts));
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.HasRightAsync(
            expectedTenantId,
            entitymetadataMock.Object,
            new Dictionary<string, object> { { "right", new string[] { "view", "add" } } },
            "edit",
            new { Id = Guid.NewGuid() },
            true
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void HasRightAsync_ShouldReturnTrue_WhenUserinfoContainsRight()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        
        // Arrange
        var customScripts = new EntityCustomScripts()
        {
            ExtendedRightsCheck = "return userinfo.right.includes(right);"
        };
        
        var entitymetadataMock = new Mock<EntityMetadata>();
        entitymetadataMock
            .Setup(m => m.CustomScripts)
            .Returns(JsonSerializer.Serialize(customScripts));
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.HasRightAsync(
            expectedTenantId,
            entitymetadataMock.Object,
            new Dictionary<string, object> { { "right", new string[] { "add", "delete" } } },
            "edit",
            new { Id = Guid.NewGuid() },
            true
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
}