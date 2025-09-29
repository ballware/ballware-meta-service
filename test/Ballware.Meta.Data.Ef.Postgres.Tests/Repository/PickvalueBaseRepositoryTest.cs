using System.Collections.Immutable;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace Ballware.Meta.Data.Ef.Postgres.Tests.Repository;

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
}