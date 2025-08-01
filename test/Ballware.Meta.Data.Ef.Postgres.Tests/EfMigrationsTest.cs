using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.Postgres.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Postgres.Tests;

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

        PreparedBuilder.Services.AddBallwareMetaStorageForPostgres(storageOptions, connectionString);
        PreparedBuilder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });

        var app = PreparedBuilder.Build();

        await app.StartAsync();
        await app.StopAsync();
    }
}