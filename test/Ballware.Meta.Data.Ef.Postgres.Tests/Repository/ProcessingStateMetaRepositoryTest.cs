using System.Collections.Immutable;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace Ballware.Meta.Data.Ef.Postgres.Tests.Repository;

public class ProcessingStateMetaRepositoryTest : RepositoryBaseTest
{
    [Test]
    public async Task GetStateAvailability_succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        using var scope = Application.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IProcessingStateMetaRepository>();
        
        var expectedList = new List<string>()
        {
            "entity1",
            "entity2",
        };

        var expectedEntries = new List<ProcessingStateSelectListEntry>();
        
        foreach (var enity in expectedList)
        {
            for (var i = 0; i < 10; i++)
            {
                var state = await repository.NewAsync(TenantId, "primary", ImmutableDictionary<string, object>.Empty);
                
                state.Entity = enity;
                state.State = i;
                state.Name = $"State {i}";
                
                await repository.SaveAsync(TenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, state);
                
                if ("entity1".Equals(state.Entity))
                {
                    expectedEntries.Add(new ProcessingStateSelectListEntry
                    {
                        Id = state.Id,
                        Name = state.Name,
                        State = state.State
                    });
                }
            }
        }
        
        // Act
        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        
        var actualList = (await repository.GetProcessingStateAvailabilityAsync(TenantId))
            .Where(entry => expectedList.Contains(entry))
            .ToList();
        
        var actualEntries = (await dbContext.Database.GetDbConnection().QueryAsync<ProcessingStateSelectListEntry>(await repository.GenerateAvailableQueryAsync(TenantId, "entity1"))).ToList();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualList.Count, Is.EqualTo(expectedList.Count));
            Assert.That(actualList, Is.EquivalentTo(expectedList));
            
            Assert.That(actualEntries.Count, Is.EqualTo(expectedEntries.Count));

            foreach (var (e, a) in expectedEntries.Zip(actualEntries))
            {
                Assert.That(a.Id, Is.EqualTo(e.Id));
                Assert.That(a.State, Is.EqualTo(e.State));
                Assert.That(a.Name, Is.EqualTo(e.Name));
            }
        });
    }
    
    [Test]
    public async Task Execute_generated_list_query_succeeds()
    {
        using var scope = Application.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IProcessingStateMetaRepository>();

        var listQuery = await repository.GenerateListQueryAsync(TenantId);

        var connection = dbContext.Database.GetDbConnection();
        
        var result = await connection.QueryAsync(listQuery);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(13));
        });
    }
}