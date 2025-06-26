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
            new Dictionary<string, object> { { "right", new string[] { "add", "edit" } } },
            "edit",
            new { Id = Guid.NewGuid() },
            true
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void StateAllowedAsync_ShouldReturnFalse_WhenStateAllowedScriptIsEmpty()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var expectedCurrentState = 1;

        var entityMetadata = new EntityMetadata()
        {
            StateAllowedScript = null
        };
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.StateAllowedAsync(
            expectedTenantId,
            entityMetadata,
            expectedId,
            expectedCurrentState,
            ["edit", "view"]
        ).Result;
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void StateAllowedAsync_ShouldReturnTrue_WhenUserHasDedicatedRight()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var expectedCurrentState = 1;

        var entityMetadata = new EntityMetadata()
        {
            StateAllowedScript = "return hasRight('entity.edit');"
        };
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.StateAllowedAsync(
            expectedTenantId,
            entityMetadata,
            expectedId,
            expectedCurrentState,
            ["entity.edit", "entity.view"]
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void StateAllowedAsync_ShouldReturnTrue_WhenUserHasAnyRight()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var expectedCurrentState = 1;

        var entityMetadata = new EntityMetadata()
        {
            StateAllowedScript = "return hasAnyRight('entity');"
        };
        
        // Act
        var rightsChecker = new JavascriptEntityRightsChecker();
        
        var result = rightsChecker.StateAllowedAsync(
            expectedTenantId,
            entityMetadata,
            expectedId,
            expectedCurrentState,
            ["entity.edit", "entity.view"]
        ).Result;
        
        // Assert
        Assert.That(result, Is.True);
    }
}