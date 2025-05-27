using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Caching.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTest
{
    [Test]
    public void AddBallwareDistributedCaching_succeeds()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddLogging();
        services.AddDistributedMemoryCache();
        services.AddBallwareDistributedCaching();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        var cache = serviceProvider.GetService<ITenantAwareEntityCache>();
        
        Assert.That(cache, Is.Not.Null);
    }
}