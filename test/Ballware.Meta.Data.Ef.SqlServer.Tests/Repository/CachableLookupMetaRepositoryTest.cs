using System.Collections.Immutable;
using System.Text;
using System.Text.Unicode;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class CachableLookupMetaRepositoryTest : RepositoryBaseTest
{
    protected override string AdditionalSettingsFile => "appsettings.withcache.json";
    
    [Test]
    public async Task Save_and_remove_value_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ILookupMetaRepository>();

        var expectedValue = await repository.NewQueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        
        expectedValue.Identifier = "fake_lookup";
        expectedValue.Name = "fake_name";
        expectedValue.ListQuery = "fake_list_query";
        expectedValue.ByIdQuery = "fake_by_id_query";
        
        await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, expectedValue);

        var actualValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);
        var actualByIdValue = await repository.ByIdAsync(TenantId, expectedValue.Id);
        var actualByIdentifierValue = await repository.ByIdentifierAsync(TenantId, expectedValue.Identifier);

        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualValue?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualValue?.ListQuery, Is.EqualTo(expectedValue.ListQuery));
            Assert.That(actualValue?.ByIdQuery, Is.EqualTo(expectedValue.ByIdQuery));
            
            Assert.That(actualByIdValue, Is.Not.Null);
            Assert.That(actualByIdValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdValue?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualByIdValue?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualByIdValue?.ListQuery, Is.EqualTo(expectedValue.ListQuery));
            Assert.That(actualByIdValue?.ByIdQuery, Is.EqualTo(expectedValue.ByIdQuery));
            
            Assert.That(actualByIdentifierValue, Is.Not.Null);
            Assert.That(actualByIdentifierValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdentifierValue?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualByIdentifierValue?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualByIdentifierValue?.ListQuery, Is.EqualTo(expectedValue.ListQuery));
            Assert.That(actualByIdentifierValue?.ByIdQuery, Is.EqualTo(expectedValue.ByIdQuery));
        });

        var removeParams = new Dictionary<string, object>([new KeyValuePair<string, object>("Id", expectedValue.Id)]);

        var removeResult = await repository.RemoveAsync(TenantId, null, ImmutableDictionary<string, object>.Empty, removeParams);

        Assert.Multiple(() =>
        {
            Assert.That(removeResult.Result, Is.True);
        });

        actualValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.That(actualValue, Is.Null);
    }
}
