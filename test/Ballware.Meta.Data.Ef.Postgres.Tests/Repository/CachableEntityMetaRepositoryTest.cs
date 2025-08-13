using System.Collections.Immutable;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Postgres.Tests.Repository;

public class CachableEntityMetaRepositoryTest : RepositoryBaseTest
{
    protected override string AdditionalSettingsFile => "appsettings.withcache.json";

    [Test]
    public async Task Save_and_remove_value_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IEntityMetaRepository>();

        var expectedValue = await repository.NewQueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        
        expectedValue.Entity = "fake_entity";
        expectedValue.Application = "fake_application";
        expectedValue.DisplayName = "fake_display_name";
        
        await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, expectedValue);

        var actualByIdValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);
        var actualByIdentifierValue = await repository.ByEntityAsync(TenantId, expectedValue.Entity);

        Assert.Multiple(() =>
        {
            Assert.That(actualByIdValue, Is.Not.Null);
            Assert.That(actualByIdValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdValue?.Entity, Is.EqualTo(expectedValue.Entity));
            Assert.That(actualByIdValue?.Application, Is.EqualTo(expectedValue.Application));
            Assert.That(actualByIdValue?.DisplayName, Is.EqualTo(expectedValue.DisplayName));
            
            Assert.That(actualByIdentifierValue, Is.Not.Null);
            Assert.That(actualByIdentifierValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdentifierValue?.Entity, Is.EqualTo(expectedValue.Entity));
            Assert.That(actualByIdentifierValue?.Application, Is.EqualTo(expectedValue.Application));
            Assert.That(actualByIdentifierValue?.DisplayName, Is.EqualTo(expectedValue.DisplayName));
        });

        var removeParams = new Dictionary<string, object>([new KeyValuePair<string, object>("Id", expectedValue.Id)]);

        var removeResult = await repository.RemoveAsync(TenantId, null, ImmutableDictionary<string, object>.Empty, removeParams);

        Assert.Multiple(() =>
        {
            Assert.That(removeResult.Result, Is.True);
        });

        actualByIdValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.That(actualByIdValue, Is.Null);
    }
}
