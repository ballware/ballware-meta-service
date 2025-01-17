using Ballware.Meta.Data.Ef.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Tests.Repository;

public class RepositoryBaseTest
{
    protected Guid TenantId { get; private set; }

    protected WebApplication Application { get; private set; }

    [OneTimeSetUp]
    public void SetupApplication()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings_with_migrations.json"), optional: false);
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.{builder.Environment.EnvironmentName}.json"), true, true);
        builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.local.json"), true, true);
        builder.Configuration.AddEnvironmentVariables();

        var storageOptions = builder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = builder.Configuration.GetConnectionString("MetaConnection");

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");

        builder.Services.AddBallwareMetaStorage(storageOptions, connectionString);
        builder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });

        Application = builder.Build();
    }

    [OneTimeTearDown]
    public async Task TearDownApplication()
    {
        await Application.DisposeAsync();
    }

    [SetUp]
    public async Task SetupTenantId()
    {
        TenantId = Guid.NewGuid();

        using var scope = Application.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();

        await seeder.SeedCustomerTenantAsync(TenantId, $"Customer_{TenantId.ToString()}");
    }
}