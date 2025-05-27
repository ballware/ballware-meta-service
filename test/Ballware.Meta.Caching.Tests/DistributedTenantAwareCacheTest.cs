using System.Text;
using Ballware.Meta.Caching.Configuration;
using Ballware.Meta.Caching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace Ballware.Meta.Caching.Tests;

class TestItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}

[TestFixture]
public class DistributedTenantAwareCacheTest
{
    private Mock<IDistributedCache> DistributedCacheMock { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        DistributedCacheMock = new Mock<IDistributedCache>();
    }
    
    [Test]
    public void TestCacheOperations_succeeds()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var entity = "TestEntity";
        var key = Guid.NewGuid().ToString();
        
        var expectedKey = $"{tenantId}_{entity}_{key}".ToLowerInvariant();
        var expectedItem = new TestItem { Id = Guid.NewGuid(), Name = entity };
        var expectedSerializedItem = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedItem));

        var options = Options.Create(new CacheOptions()
        {
            CacheExpirationHours = 1
        });
        
        // Mock the distributed cache methods
        DistributedCacheMock.Setup(c => c.Get(expectedKey))
            .Returns(expectedSerializedItem);

        DistributedCacheMock.Setup(cache =>
                cache.Set(expectedKey, expectedSerializedItem, It.IsAny<DistributedCacheEntryOptions>()))
            .Callback((string key, byte[] item, DistributedCacheEntryOptions options) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(key, Is.EqualTo(expectedKey));
                    Assert.That(item, Is.EqualTo(expectedSerializedItem));
                    Assert.That(options.AbsoluteExpirationRelativeToNow, Is.EqualTo(TimeSpan.FromHours(1)));
                });
            });
        
        var cache = new DistributedTenantAwareCache(
            new LoggerFactory().CreateLogger<DistributedTenantAwareCache>(),
            DistributedCacheMock.Object,
            options
        );
        
        // Act
        cache.SetItem(tenantId, entity, key, expectedItem);
        
        Assert.That(cache.TryGetItem<TestItem>(tenantId, entity, key, out TestItem? cachedItem), Is.True);
        
        cache.PurgeItem(tenantId, entity, key);
        
        DistributedCacheMock.Setup(c => c.Get(expectedKey))
            .Returns(null as byte[]);
        
        Assert.That(cache.TryGetItem<TestItem>(tenantId, entity, key, out TestItem? uncachedItem), Is.False);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(uncachedItem, Is.Null);
            Assert.That(cachedItem, Is.Not.Null);
            Assert.That(cachedItem!.Id, Is.EqualTo(expectedItem.Id));
            Assert.That(cachedItem.Name, Is.EqualTo(expectedItem.Name));
        });
        
        DistributedCacheMock.Verify(c => c.Set(expectedKey, expectedSerializedItem, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
        DistributedCacheMock.Verify(c => c.Get(expectedKey), Times.Exactly(2));
        DistributedCacheMock.Verify(c => c.Remove(expectedKey), Times.Once);
        
    }
}