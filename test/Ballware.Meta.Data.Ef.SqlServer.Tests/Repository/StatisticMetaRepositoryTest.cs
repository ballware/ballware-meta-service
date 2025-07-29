using System.Collections.Immutable;
using System.Text;
using Ballware.Meta.Data.Ef.SqlServer;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class StatisticMetaRepositoryTest : RepositoryBaseTest
{
    [Test]
    public async Task Save_and_remove_value_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IStatisticMetaRepository>();

        var expectedValue = await repository.NewQueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);

        expectedValue.Identifier = $"fake_identifier_1";
        expectedValue.Entity = "fake_entity";
        expectedValue.Name = "fake_name_1";
        expectedValue.Layout = "{}";
        expectedValue.FetchSql = "fake sql";
        expectedValue.FetchScript = "fake fetch script";
        expectedValue.MappingScript = "fake mapping script";
        expectedValue.CustomScripts = "{}";

        await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, expectedValue);

        var actualValue = await repository.ByIdAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, expectedValue.Id);
        var actualByIdentifier = await repository.MetadataByIdentifierAsync(TenantId, expectedValue.Identifier);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualValue, Is.Not.Null);
            Assert.That(actualValue?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualValue?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualValue?.Entity, Is.EqualTo(expectedValue.Entity));
            Assert.That(actualValue?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualValue?.Layout, Is.EqualTo(expectedValue.Layout));
            Assert.That(actualValue?.FetchSql, Is.EqualTo(expectedValue.FetchSql));
            Assert.That(actualValue?.FetchScript, Is.EqualTo(expectedValue.FetchScript));
            Assert.That(actualValue?.MappingScript, Is.EqualTo(expectedValue.MappingScript));
            Assert.That(actualValue?.CustomScripts, Is.EqualTo(expectedValue.CustomScripts));
            
            Assert.That(actualByIdentifier, Is.Not.Null);
            Assert.That(actualByIdentifier?.Id, Is.EqualTo(expectedValue.Id));
            Assert.That(actualByIdentifier?.Identifier, Is.EqualTo(expectedValue.Identifier));
            Assert.That(actualByIdentifier?.Entity, Is.EqualTo(expectedValue.Entity));
            Assert.That(actualByIdentifier?.Name, Is.EqualTo(expectedValue.Name));
            Assert.That(actualByIdentifier?.Layout, Is.EqualTo(expectedValue.Layout));
            Assert.That(actualByIdentifier?.FetchSql, Is.EqualTo(expectedValue.FetchSql));
            Assert.That(actualByIdentifier?.FetchScript, Is.EqualTo(expectedValue.FetchScript));
            Assert.That(actualByIdentifier?.MappingScript, Is.EqualTo(expectedValue.MappingScript));
            Assert.That(actualByIdentifier?.CustomScripts, Is.EqualTo(expectedValue.CustomScripts));
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

        var repository = scope.ServiceProvider.GetRequiredService<IStatisticMetaRepository>();

        var fakeTenantIds = new[] { Guid.NewGuid(), Guid.NewGuid(), TenantId, Guid.NewGuid() };

        List<Guid> fakeValueIds = new List<Guid>();
        
        foreach (var fakeTenant in fakeTenantIds)
        {
            for (var i = 0; i < 10; i++)
            {
                var fakeValue = await repository.NewAsync(fakeTenant, "primary", ImmutableDictionary<string, object>.Empty);

                fakeValue.Identifier = $"fake_identifier_{i}";
                fakeValue.Entity = "fake_entity";
                fakeValue.Name = $"fake_name_{i}";
                fakeValue.Layout = "{}";
                fakeValue.FetchSql = "fake sql";
                fakeValue.FetchScript = "fake fetch script";
                fakeValue.MappingScript = "fake mapping script";
                fakeValue.CustomScripts = "{}";
                
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
        });
    }

    [Test]
    public async Task Import_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IStatisticMetaRepository>();

        var importList = new List<Statistic>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Identifier = $"fake_identifier_{i}";
            fakeValue.Entity = "fake_entity";
            fakeValue.Name = $"fake_name_{i}";
            fakeValue.Layout = "{}";
            fakeValue.FetchSql = "fake sql";
            fakeValue.FetchScript = "fake fetch script";
            fakeValue.MappingScript = "fake mapping script";
            fakeValue.CustomScripts = "{}";
            
            importList.Add(fakeValue);
        }

        var importBinary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(importList));

        using var importStream = new MemoryStream(importBinary);

        await repository.ImportAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, importStream, (doc) => Task.FromResult(true));

        var actualTenantItemsCount = await repository.CountAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        var actualTenantAllItems = await repository.AllAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        var actualTenantQueryItems = await repository.QueryAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, ImmutableDictionary<string, object>.Empty);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantItemsCount, Is.EqualTo(10));
            Assert.That(actualTenantAllItems.Count(), Is.EqualTo(10));
            Assert.That(actualTenantQueryItems.Count(), Is.EqualTo(10));
        });
    }

    [Test]
    public async Task Export_values_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IStatisticMetaRepository>();

        var exportIdList = new List<Guid>();
        var exportItemList = new List<Statistic>();

        for (var i = 0; i < 10; i++)
        {
            var fakeValue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);

            fakeValue.Identifier = $"fake_identifier_{i}";
            fakeValue.Entity = "fake_entity";
            fakeValue.Name = $"fake_name_{i}";
            fakeValue.Layout = "{}";
            fakeValue.FetchSql = "fake sql";
            fakeValue.FetchScript = "fake fetch script";
            fakeValue.MappingScript = "fake mapping script";
            fakeValue.CustomScripts = "{}";
            
            await repository.SaveAsync(TenantId, null, "primary", ImmutableDictionary<string, object>.Empty, fakeValue);

            if (i % 2 == 0)
            {
                exportIdList.Add(fakeValue.Id);
                exportItemList.Add(fakeValue);
            }
        }

        var idStringValues = new StringValues(exportIdList.Select(id => id.ToString()).ToArray());
        
        var exportResult = await repository.ExportAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>(new[] { new KeyValuePair<string, object>("id", idStringValues) }));

        Assert.Multiple(() =>
        {
            Assert.That(exportResult.FileName, Is.EqualTo("primary.json"));
            Assert.That(exportResult.MediaType, Is.EqualTo("application/json"));
            Assert.That(exportResult.Data, Is.Not.Null);

            using var inputStream = new MemoryStream(exportResult.Data);
            using var streamReader = new StreamReader(inputStream);

            var actualItems = JsonConvert.DeserializeObject<IEnumerable<Documentation>>(streamReader.ReadToEnd())?.ToList();

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
        var repository = scope.ServiceProvider.GetRequiredService<IStatisticMetaRepository>();

        var listQuery = await repository.GenerateListQueryAsync(TenantId);

        var connection = dbContext.Database.GetDbConnection();
        
        var result = await connection.QueryAsync(listQuery);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(0));
        });
    }
}