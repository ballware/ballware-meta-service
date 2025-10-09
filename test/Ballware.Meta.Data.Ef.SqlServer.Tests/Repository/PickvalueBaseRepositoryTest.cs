using System.Collections.Immutable;
using Ballware.Meta.Data.Ef.SqlServer;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class PickvalueBaseRepositoryTest : RepositoryBaseTest
{
    [Test]
    public async Task GetPickvalueAvailability_succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPickvalueMetaRepository>();
        
        var expectedList = new List<PickvalueAvailability>()
        {
            new() { Entity = "entity1", Field = "field1" },
            new() { Entity = "entity1", Field = "field2" },
            new() { Entity = "entity2", Field = "field1" },
        };

        var expectedEntries = new List<PickvalueSelectEntry>();
        
        foreach (var entityField in expectedList)
        {
            for (var i = 0; i < 10; i++)
            {
                var pickvalue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
                
                pickvalue.Entity = entityField.Entity;
                pickvalue.Field = entityField.Field;
                pickvalue.Value = i;
                pickvalue.Text = $"Label {i}";
                pickvalue.Sorting = i;
                
                await repository.SaveAsync(TenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, pickvalue);
                
                if ("entity1".Equals(pickvalue.Entity) && "field2".Equals(pickvalue.Field))
                {
                    expectedEntries.Add(new PickvalueSelectEntry
                    {
                        Id = pickvalue.Id,
                        Name = pickvalue.Text,
                        Value = pickvalue.Value
                    });
                }
            }
        }
        
        // Act
        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        
        var relevantEntities = expectedList.Select(entry => entry.Entity).Distinct().ToList();
        
        var actualList = (await repository.GetPickvalueAvailabilityAsync(TenantId))
            .Where(entry => relevantEntities.Contains(entry.Entity))
            .ToList();
        
        var actualEntries = (await dbContext.Database.GetDbConnection().QueryAsync<PickvalueSelectEntry>(await repository.GenerateAvailableQueryAsync(TenantId, "entity1", "field2"))).ToList();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualList.Count, Is.EqualTo(expectedList.Count));

            foreach (var (e, a) in expectedList.Zip(actualList))
            {
                Assert.That(a.Entity, Is.EqualTo(e.Entity));
                Assert.That(a.Field, Is.EqualTo(e.Field));
            }
            
            Assert.That(actualEntries.Count, Is.EqualTo(expectedEntries.Count));

            foreach (var (e, a) in expectedEntries.Zip(actualEntries))
            {
                Assert.That(a.Id, Is.EqualTo(e.Id));
                Assert.That(a.Value, Is.EqualTo(e.Value));
                Assert.That(a.Name, Is.EqualTo(e.Name));
            }
        });
    }
    
    [Test]
    public async Task GetPickvalueSpecialQueries_succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPickvalueMetaRepository>();
        
        var expectedList = new List<PickvalueAvailability>()
        {
            new() { Entity = "entity1", Field = "field1" },
            new() { Entity = "entity1", Field = "field2" },
            new() { Entity = "entity2", Field = "field1" },
        };

        var expectedEntityEntries = new List<Pickvalue>();
        var expectedFieldEntries = new List<Pickvalue>();
        
        foreach (var entityField in expectedList)
        {
            for (var i = 0; i < 10; i++)
            {
                var pickvalue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
                
                pickvalue.Entity = entityField.Entity;
                pickvalue.Field = entityField.Field;
                pickvalue.Value = i;
                pickvalue.Text = $"Label {i}";
                pickvalue.Sorting = i;
                
                await repository.SaveAsync(TenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, pickvalue);

                if ("entity1".Equals(pickvalue.Entity))
                {
                    expectedEntityEntries.Add(pickvalue);
                }
                
                if ("entity1".Equals(pickvalue.Entity) && "field2".Equals(pickvalue.Field))
                {
                    expectedFieldEntries.Add(pickvalue);
                }
            }
        }
        
        // Act
        Assert.ThrowsAsync<ArgumentException>(async () => await repository.QueryAsync(TenantId, "entity",
            ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()));
        
        var actualEntityEntries = (await repository.QueryAsync(TenantId, "entity", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "entity1" }
        })).ToList();
        
        Assert.ThrowsAsync<ArgumentException>(async () => await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "entity1" },
        }));
        
        Assert.ThrowsAsync<ArgumentException>(async () => await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()));
        
        var actualFieldEntries = (await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "entity1" },
            { "field", "field2" },
            { "id", expectedFieldEntries.Select(e => e.Id.ToString()).ToArray() }
        })).ToList();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualEntityEntries.Count, Is.EqualTo(expectedEntityEntries.Count));
            
            foreach (var (e, a) in expectedEntityEntries.Zip(actualEntityEntries))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(a.Id, Is.EqualTo(e.Id));
                    Assert.That(a.Entity, Is.EqualTo(e.Entity));
                    Assert.That(a.Field, Is.EqualTo(e.Field));
                    Assert.That(a.Value, Is.EqualTo(e.Value));
                    Assert.That(a.Text, Is.EqualTo(e.Text));
                    Assert.That(a.Sorting, Is.EqualTo(e.Sorting));
                });
            }
            
            Assert.That(actualFieldEntries.Count, Is.EqualTo(expectedFieldEntries.Count));
            
            foreach (var (e, a) in expectedFieldEntries.Zip(actualFieldEntries))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(a.Id, Is.EqualTo(e.Id));
                    Assert.That(a.Entity, Is.EqualTo(e.Entity));
                    Assert.That(a.Field, Is.EqualTo(e.Field));
                    Assert.That(a.Value, Is.EqualTo(e.Value));
                    Assert.That(a.Text, Is.EqualTo(e.Text));
                    Assert.That(a.Sorting, Is.EqualTo(e.Sorting));
                });
            }
        });
    }
    
    [Test]
    public async Task Execute_generated_list_query_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IPickvalueMetaRepository>();

        var listQuery = await repository.GenerateListQueryAsync(TenantId);

        var connection = dbContext.Database.GetDbConnection();
        
        var result = await connection.QueryAsync(listQuery);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(21));
        });
    }
    
    [Test]
    public async Task GetPickvalue_WithCaseSensitiveField_succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPickvalueMetaRepository>();
        
        // Create pickvalue with lowercase field name
        var pickvalue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        pickvalue.Entity = "testentity";
        pickvalue.Field = "category"; // lowercase field
        pickvalue.Value = 1;
        pickvalue.Text = "Test Category";
        pickvalue.Sorting = 1;
        
        await repository.SaveAsync(TenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, pickvalue);
        
        // Act - Query with uppercase field name
        var resultWithUpperCase = await repository.SelectListForEntityFieldAsync(TenantId, "testentity", "Category");
        var resultWithLowerCase = await repository.SelectListForEntityFieldAsync(TenantId, "testentity", "category");
        var resultWithMixedCase = await repository.SelectListForEntityFieldAsync(TenantId, "testentity", "CaTeGoRy");
        
        var selectByValueUpper = await repository.SelectByValueAsync(TenantId, "testentity", "Category", 1);
        var selectByValueLower = await repository.SelectByValueAsync(TenantId, "testentity", "category", 1);
        var selectByValueMixed = await repository.SelectByValueAsync(TenantId, "testentity", "CaTeGoRy", 1);
        
        // Assert - All case variations should return the same result
        Assert.Multiple(() =>
        {
            Assert.That(resultWithUpperCase.Count(), Is.EqualTo(1), "Upper case field should find entry");
            Assert.That(resultWithLowerCase.Count(), Is.EqualTo(1), "Lower case field should find entry");
            Assert.That(resultWithMixedCase.Count(), Is.EqualTo(1), "Mixed case field should find entry");
            
            Assert.That(selectByValueUpper, Is.Not.Null, "Upper case field should find entry by value");
            Assert.That(selectByValueLower, Is.Not.Null, "Lower case field should find entry by value");
            Assert.That(selectByValueMixed, Is.Not.Null, "Mixed case field should find entry by value");
            
            Assert.That(selectByValueUpper!.Value, Is.EqualTo(1));
            Assert.That(selectByValueLower!.Value, Is.EqualTo(1));
            Assert.That(selectByValueMixed!.Value, Is.EqualTo(1));
        });
    }
    
    [Test]
    public async Task QueryPickvalue_WithCaseSensitiveEntityAndField_succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IPickvalueMetaRepository>();
        
        // Create pickvalue with lowercase entity and field names
        var pickvalue = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
        pickvalue.Entity = "myentity"; // lowercase entity
        pickvalue.Field = "myfield"; // lowercase field
        pickvalue.Value = 1;
        pickvalue.Text = "Test Value";
        pickvalue.Sorting = 1;
        
        await repository.SaveAsync(TenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, pickvalue);
        
        // Act - Query with different case variations
        var resultLowerCase = (await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "myentity" },
            { "field", "myfield" }
        })).ToList();
        
        var resultUpperCase = (await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "MyEntity" },
            { "field", "MyField" }
        })).ToList();
        
        var resultMixedCase = (await repository.QueryAsync(TenantId, "entityandfield", ImmutableDictionary<string, object>.Empty, new Dictionary<string, object>()
        {
            { "entity", "MYENTITY" },
            { "field", "MYFIELD" }
        })).ToList();
        
        // Assert - All case variations should return the same result
        Assert.Multiple(() =>
        {
            Assert.That(resultLowerCase.Count, Is.EqualTo(1), "Lower case entity/field should find entry");
            Assert.That(resultUpperCase.Count, Is.EqualTo(1), "Upper case entity/field should find entry");
            Assert.That(resultMixedCase.Count, Is.EqualTo(1), "Mixed case entity/field should find entry");
            
            Assert.That(resultLowerCase[0].Value, Is.EqualTo(1));
            Assert.That(resultUpperCase[0].Value, Is.EqualTo(1));
            Assert.That(resultMixedCase[0].Value, Is.EqualTo(1));
        });
    }
}