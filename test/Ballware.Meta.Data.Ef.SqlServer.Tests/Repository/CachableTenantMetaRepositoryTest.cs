using System.Collections.Immutable;
using System.Text;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class CachableTenantMetaRepositoryTest : RepositoryBaseTest
{
    protected override string AdditionalSettingsFile => "appsettings.withcache.json";
    
    [Test]
    public async Task Save_and_remove_value_succeeds()
    {
        var fakeTenantId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();

        var expectedValue = await repository.NewQueryAsync(fakeTenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);

        expectedValue.Provider = "mssql";
        expectedValue.Name = "fake_tenant";
        
        await repository.SaveAsync(fakeTenantId, null, "primary", ImmutableDictionary<string, object>.Empty, expectedValue);

        var actualValue = await repository.ByIdAsync(fakeTenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);
        var actualByIdValue = await repository.ByIdAsync(expectedValue.Id);

        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Provider, Is.EqualTo(expectedValue.Provider));
            Assert.That(actualValue?.Name, Is.EqualTo(expectedValue.Name));
            
            Assert.That(actualByIdValue, Is.Not.Null);
            Assert.That(actualByIdValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdValue?.Provider, Is.EqualTo(expectedValue.Provider));
            Assert.That(actualByIdValue?.Name, Is.EqualTo(expectedValue.Name));
        });

        var removeParams = new Dictionary<string, object>([new KeyValuePair<string, object>("Id", expectedValue.Id)]);

        var removeResult = await repository.RemoveAsync(fakeTenantId, null, ImmutableDictionary<string, object>.Empty, removeParams);

        Assert.Multiple(() =>
        {
            Assert.That(removeResult.Result, Is.True);
        });

        actualValue = await repository.ByIdAsync(fakeTenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.That(actualValue, Is.Null);
    }
}
