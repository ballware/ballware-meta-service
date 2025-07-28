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

public class EntityMetaRepositoryTest : RepositoryBaseTest
{
    private const int MetaEntityCount = 15;
    
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

        var actualValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);

        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Entity, Is.EqualTo(expectedValue.Entity));
            Assert.That(actualValue?.Application, Is.EqualTo(expectedValue.Application));
            Assert.That(actualValue?.DisplayName, Is.EqualTo(expectedValue.DisplayName));
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

    [Test]
    public async Task Query_tenant_items_succeeds()
    {
        using var scope = Application.Services.CreateScope();
        
        var repository = scope.ServiceProvider.GetRequiredService<IEntityMetaRepository>();

        var fakeTenantIds = new[] { Guid.NewGuid(), Guid.NewGuid(), TenantId, Guid.NewGuid() };

        List<Guid> fakeValueIds = new List<Guid>();
        
        foreach (var fakeTenant in fakeTenantIds)
        {
            for (var i = 0; i < 10; i++)
            {
                var fakeValue = await repository.NewAsync(fakeTenant, "primary", ImmutableDictionary<string, object>.Empty);

                fakeValue.Entity = $"fake_entity_{fakeTenant.ToString()}_{i}";
                fakeValue.Application = $"fake_application";
                fakeValue.DisplayName = $"fake_displayname_{i}";
                
                await repository.SaveAsync(fakeTenant, null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);

                if (fakeTenant == TenantId)
                {
                    fakeValueIds.Add(fakeValue.Id);    
                }
            }
        }

        var actualTenantItemsCount = await repository.CountAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualEntityItem = await repository.ByEntityAsync(TenantId, $"fake_entity_{TenantId.ToString()}_1");

        var actualSelectListItems = await repository.SelectListForTenantAsync(TenantId);
        var actualSelectByIdItem = await repository.SelectByIdForTenantAsync(TenantId, fakeValueIds[0]);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualSelectListItems.Count(), Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualSelectByIdItem, Is.Not.Null);
            Assert.That(actualSelectByIdItem?.Id, Is.EqualTo(fakeValueIds[0]));
            Assert.That(actualEntityItem?.Entity, Is.EqualTo($"fake_entity_{TenantId.ToString()}_1"));
            Assert.That(actualEntityItem?.Application, Is.EqualTo("fake_application"));
            Assert.That(actualEntityItem?.DisplayName, Is.EqualTo("fake_displayname_1"));
        });
    }

    [Test]
    public async Task Import_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IEntityMetaRepository>();

        var importList = new List<EntityMetadata>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Entity = $"fake_imports_{TenantId.ToString()}_{i}";
            fakeValue.Application = $"fake_application_{i}";
            fakeValue.DisplayName = $"fake_displayname_{i}";

            importList.Add(fakeValue);
        }

        var importBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(importList));

        using var importStream = new MemoryStream(importBinary);

        await repository.ImportAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, importStream, (doc) => Task.FromResult(true));

        var actualTenantItemsCount = await repository.CountAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualEntityItem = await repository.ByEntityAsync(TenantId, $"fake_imports_{TenantId.ToString()}_1");

        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(10 + MetaEntityCount));
            Assert.That(actualEntityItem?.Entity, Is.EqualTo($"fake_imports_{TenantId.ToString()}_1"));
            Assert.That(actualEntityItem?.Application, Is.EqualTo("fake_application_1"));
            Assert.That(actualEntityItem?.DisplayName, Is.EqualTo("fake_displayname_1"));
        });
    }

    [Test]
    public async Task Export_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IEntityMetaRepository>();

        var exportIdList = new List<Guid>();
        var exportItemList = new List<EntityMetadata>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Entity = $"fake_exports_{TenantId.ToString()}_{i}";
            fakeValue.Application = $"fake_application_{i}";
            fakeValue.DisplayName = $"fake_displayname_{i}";

            await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);

            if (i % 2 == 0)
            {
                exportIdList.Add(fakeValue.Id);
                exportItemList.Add(fakeValue);
            }
        }

        var exportResult = await repository.ExportAsync(TenantId, "exportjson", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>(new[] { new KeyValuePair<string, object>("id", exportIdList.Select(id => id.ToString()).ToArray()) }));

        Assert.Multiple(() =>
        {
            Assert.That(exportResult.FileName, Is.EqualTo("exportjson.json"));
            Assert.That(exportResult.MediaType, Is.EqualTo("application/json"));
            Assert.That(exportResult.Data, Is.Not.Null);

            using var inputStream = new MemoryStream(exportResult.Data);
            using var streamReader = new StreamReader(inputStream);

            var actualItems = JsonConvert.DeserializeObject<IEnumerable<EntityMetadata>>(streamReader.ReadToEnd())?.ToList();

            Assert.That(actualItems, Is.Not.Null);
            Assert.That(actualItems?.Count, Is.EqualTo(5));
            Assert.That(actualItems?.Select(item => item.Id), Is.EquivalentTo(exportItemList.Select(item => item.Id)));
        });
    }
}
