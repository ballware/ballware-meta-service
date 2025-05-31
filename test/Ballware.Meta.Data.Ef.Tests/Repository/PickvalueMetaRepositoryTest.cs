using System.Collections.Immutable;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class PickvalueMetaRepositoryTest : RepositoryBaseTest
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
        
        var actualList = (await repository.GetPickvalueAvailabilityAsync(TenantId)).ToList();
        
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
}