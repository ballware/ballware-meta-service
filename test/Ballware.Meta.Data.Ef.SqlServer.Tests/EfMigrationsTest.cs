using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.SqlServer;
using Ballware.Meta.Data.Ef.Tests.Utils;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Tests;

[TestFixture]
public class EfMigrationsTest : DatabaseBackedBaseTest
{
    [Test]
    public async Task Initialization_with_migrations_up_succeeds()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = MasterConnectionString;

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        PreparedBuilder.Services.AddBallwareMetaStorageForSqlServer(storageOptions, connectionString);
        
        var mapsterConfig = new TypeAdapterConfig()
            .AddBallwareStorageMappings();
        
        PreparedBuilder.Services.AddSingleton(mapsterConfig);
        PreparedBuilder.Services.AddScoped<IMapper, ServiceMapper>();
        

        var app = PreparedBuilder.Build();

        await app.StartAsync();
        await app.StopAsync();
    }
}