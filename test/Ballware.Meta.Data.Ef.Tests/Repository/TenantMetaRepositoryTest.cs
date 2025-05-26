using System.Collections.Immutable;
using System.Text;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class TenantMetaRepositoryTest : RepositoryBaseTest
{
    [Test]
    public async Task Save_and_remove_value_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();

        var expectedValue = await repository.NewQueryAsync("primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);

        expectedValue.Provider = "mssql";
        expectedValue.Name = "fake_tenant";
        
        await repository.SaveAsync(null, "primary", ImmutableDictionary<string, object>.Empty, expectedValue);

        var actualValue = await repository.ByIdAsync("primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Provider, Is.EqualTo(expectedValue.Provider));
            Assert.That(actualValue?.Name, Is.EqualTo(expectedValue.Name));
        });

        var removeParams = new Dictionary<string, object>([new KeyValuePair<string, object>("Id", expectedValue.Id)]);

        var removeResult = await repository.RemoveAsync(null, ImmutableDictionary<string, object>.Empty, removeParams);

        Assert.Multiple(() =>
        {
            Assert.That(removeResult.Result, Is.True);
        });

        actualValue = await repository.ByIdAsync("primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.That(actualValue, Is.Null);
    }

    [Test]
    public async Task Query_tenant_items_succeeds()
    {
        using var scope = Application.Services.CreateScope();
        
        var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();

        var fakeTenantIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        
        foreach (var fakeTenant in fakeTenantIds)
        {
            var fakeValue = await repository.NewAsync("primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Id = fakeTenant;
            fakeValue.Provider = "mssql";
            fakeValue.Name = $"fake_tenant_{fakeTenant.ToString()}";
            
            await repository.SaveAsync(null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);
        }

        var actualTenantItemsCount = await repository.CountAsync("primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync("primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync("primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantItem = await repository.ByIdAsync(TenantId);
        var actualAllowedTenants = await repository.AllowedTenantsAsync(new Dictionary<string, object>()
        {
            { "allowed_tenant", new [] { TenantId.ToString(), fakeTenantIds[0].ToString() } }
        });

        var actualSelectListItems = await repository.SelectListAsync();
        var actualSelectByIdItem = await repository.SelectByIdAsync(fakeTenantIds[0]);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(8));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(8));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(8));
            Assert.That(actualSelectListItems.Count(), Is.EqualTo(8));
            Assert.That(actualAllowedTenants.Count(), Is.EqualTo(2));
            Assert.That(actualSelectByIdItem, Is.Not.Null);
            Assert.That(actualSelectByIdItem?.Id, Is.EqualTo(fakeTenantIds[0]));
            Assert.That(actualTenantItem?.Name, Is.EqualTo($"Customer_{TenantId.ToString()}"));
            Assert.That(actualTenantItem?.Provider, Is.EqualTo("mssql"));
        });
    }

    [Test]
    public async Task Import_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();

        var importList = new List<Tenant>();

        var fakeValue = await repository.NewAsync("primary", ImmutableDictionary<string, object>.Empty);

        fakeValue.Id = TenantId;
        fakeValue.Provider = "mssql";
        fakeValue.Name = $"fake_imports_{TenantId.ToString()}";

        importList.Add(fakeValue);
        
        var importBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(importList));

        using var importStream = new MemoryStream(importBinary);

        await repository.ImportAsync(null, "primary", ImmutableDictionary<string, object>.Empty, importStream, (_) => Task.FromResult(true));

        var actualTenantItemsCount = await repository.CountAsync("primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync("primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync("primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantItem = await repository.ByIdAsync(TenantId);

        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(3));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(3));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(3));
            Assert.That(actualTenantItem?.Name, Is.EqualTo($"fake_imports_{TenantId.ToString()}"));
            Assert.That(actualTenantItem?.Provider, Is.EqualTo("mssql"));
        });
    }

    [Test]
    public async Task Export_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();

        var exportIdList = new List<Guid>();
        var exportItemList = new List<Tenant>();

        var fakeValue = await repository.NewAsync("primary", ImmutableDictionary<string, object>.Empty);

        fakeValue.Provider = "mssql";
        fakeValue.Name = $"fake_exports_{TenantId.ToString()}";

        await repository.SaveAsync(null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);

        exportIdList.Add(fakeValue.Id);
        exportItemList.Add(fakeValue);
        
        var exportResult = await repository.ExportAsync("primary", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>(new[] { new KeyValuePair<string, object>("id", exportIdList.Select(id => id.ToString()).ToArray()) }));

        Assert.Multiple(() =>
        {
            Assert.That(exportResult.FileName, Is.EqualTo("primary.json"));
            Assert.That(exportResult.MediaType, Is.EqualTo("application/json"));
            Assert.That(exportResult.Data, Is.Not.Null);

            using var inputStream = new MemoryStream(exportResult.Data);
            using var streamReader = new StreamReader(inputStream);

            var actualItems = JsonConvert.DeserializeObject<IEnumerable<EntityMetadata>>(streamReader.ReadToEnd())?.ToList();

            Assert.That(actualItems, Is.Not.Null);
            Assert.That(actualItems?.Count, Is.EqualTo(1));
            Assert.That(actualItems?.Select(item => item.Id), Is.EquivalentTo(exportItemList.Select(item => item.Id)));
        });
    }
}
