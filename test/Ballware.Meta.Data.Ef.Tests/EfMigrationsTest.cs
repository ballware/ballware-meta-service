using Ballware.Meta.Data.Ef.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ballware.Meta.Data.Ef.Tests;

[TestFixture]
public class EfMigrationsTest
{
    [Test]
    public async Task Initialization_with_migrations_succeeds()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings_with_migrations.json"), optional: false);
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.{builder.Environment.EnvironmentName}.json"), true, true);
        builder.Configuration.AddEnvironmentVariables();
        
        var storageOptions = builder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = builder.Configuration.GetConnectionString("MetaConnection");
        
        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });
        
        builder.Services.AddBallwareMetaStorage(storageOptions, connectionString);

        var app = builder.Build();

        await app.StartAsync();
        await app.StopAsync();
    }
}