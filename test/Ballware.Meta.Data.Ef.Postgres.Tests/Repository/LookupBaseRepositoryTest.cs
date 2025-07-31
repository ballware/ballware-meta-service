using System.Collections.Immutable;
using System.Text;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Postgres.Tests.Repository;

public class LookupBaseRepositoryTest : RepositoryBaseTest
{
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

        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualValue?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualValue?.ListQuery, Is.EqualTo(expectedValue.ListQuery));
            Assert.That(actualValue?.ByIdQuery, Is.EqualTo(expectedValue.ByIdQuery));
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

        var repository = scope.ServiceProvider.GetRequiredService<ILookupMetaRepository>();

        var fakeTenantIds = new[] { Guid.NewGuid(), Guid.NewGuid(), TenantId, Guid.NewGuid() };

        List<Guid> fakeValueIds = new List<Guid>();
        
        foreach (var fakeTenant in fakeTenantIds)
        {
            for (var i = 0; i < 10; i++)
            {
                var fakeValue = await repository.NewAsync(fakeTenant, "primary", ImmutableDictionary<string, object>.Empty);

                fakeValue.Identifier = $"fake_lookup_{fakeTenant.ToString()}_{i}";
                fakeValue.Name = $"fake_name_{i}";
                fakeValue.ListQuery = $"fake_list_query_{i}";
                fakeValue.ByIdQuery = $"fake_by_id_query_{i}";
                
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
        var actualEntityItem = await repository.ByIdentifierAsync(TenantId, $"fake_lookup_{TenantId.ToString()}_1");

        var actualSelectListItems = await repository.SelectListForTenantAsync(TenantId);
        var actualSelectByIdItem = await repository.SelectByIdForTenantAsync(TenantId, fakeValueIds[0]);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(10));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(10));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(10));
            Assert.That(actualSelectListItems.Count(), Is.EqualTo(10));
            Assert.That(actualSelectByIdItem, Is.Not.Null);
            Assert.That(actualSelectByIdItem?.Id, Is.EqualTo(fakeValueIds[0]));
            Assert.That(actualEntityItem?.Identifier, Is.EqualTo($"fake_lookup_{TenantId.ToString()}_1"));
            Assert.That(actualEntityItem?.Name, Is.EqualTo("fake_name_1"));
        });
    }

    [Test]
    public async Task Import_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ILookupMetaRepository>();

        var importList = new List<Lookup>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Identifier = $"fake_imports_{TenantId.ToString()}_{i}";
            fakeValue.Name = $"fake_name_{i}";
            fakeValue.ListQuery = $"fake_list_query_{i}";
            fakeValue.ByIdQuery = $"fake_by_id_query_{i}";

            importList.Add(fakeValue);
        }

        var importBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(importList));

        using var importStream = new MemoryStream(importBinary);

        await repository.ImportAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, importStream, (doc) => Task.FromResult(true));

        var actualTenantItemsCount = await repository.CountAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualAllForTenantItems = await repository.AllForTenantAsync(TenantId);
        var actualByIdItem = await repository.ByIdAsync(TenantId, importList[0].Id);
        var actualEntityItem = await repository.ByIdentifierAsync(TenantId, $"fake_imports_{TenantId.ToString()}_1");
        var actualSelectListItems = await repository.SelectListForTenantAsync(TenantId);
        

        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(10));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(10));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(10));
            Assert.That(actualAllForTenantItems.Count(), Is.EqualTo(10));
            Assert.That(actualSelectListItems.Count(), Is.EqualTo(10));
            Assert.That(actualByIdItem, Is.Not.Null);
            Assert.That(actualEntityItem?.Identifier, Is.EqualTo($"fake_imports_{TenantId.ToString()}_1"));
            Assert.That(actualEntityItem?.Name, Is.EqualTo("fake_name_1"));
        });
    }

    [Test]
    public async Task Export_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ILookupMetaRepository>();

        var exportIdList = new List<Guid>();
        var exportItemList = new List<Lookup>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Identifier = $"fake_exports_{TenantId.ToString()}_{i}";
            fakeValue.Name = $"fake_name_{i}";
            fakeValue.ListQuery = $"fake_list_query_{i}";
            fakeValue.ByIdQuery = $"fake_by_id_query_{i}";

            await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);

            if (i % 2 == 0)
            {
                exportIdList.Add(fakeValue.Id);
                exportItemList.Add(fakeValue);
            }
        }

        var exportResult = await repository.ExportAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>(new[] { new KeyValuePair<string, object>("id", exportIdList.Select(id => id.ToString()).ToArray()) }));

        Assert.Multiple(() =>
        {
            Assert.That(exportResult.FileName, Is.EqualTo("primary.json"));
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
    
    [Test]
    public async Task Execute_generated_list_query_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<ILookupMetaRepository>();

        var listQuery = await repository.GenerateListQueryAsync(TenantId);

        var connection = dbContext.Database.GetDbConnection();
        
        var result = await connection.QueryAsync(listQuery);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(0));
        });
    }
}
