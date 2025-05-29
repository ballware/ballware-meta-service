using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Authorization.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTest
{
    [Test]
    public void AddBallwareMetaAuthorizationUtils_succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddLogging();
        services.AddBallwareMetaAuthorizationUtils("tenant", "sub", "right");
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var principalUtils = serviceProvider.GetService<IPrincipalUtils>();
        
        Assert.Multiple(() =>
        {
            Assert.That(principalUtils, Is.Not.Null);
        });
    }
}