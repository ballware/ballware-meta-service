using System.Reflection;
using Ballware.Meta.Data.Ef.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Tests;

[TestFixture]
public class AdminTenantSeedTest
{
    private WebApplicationBuilder PreparedBuilder { get; set; } = null!;
    
    [SetUp]
    public void Setup()
    {
        PreparedBuilder = WebApplication.CreateBuilder();

        PreparedBuilder.Configuration.Sources.Clear();
        PreparedBuilder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings_with_migrations.json"), optional: false);
        PreparedBuilder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.{PreparedBuilder.Environment.EnvironmentName}.json"), true, true);
        PreparedBuilder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings_with_migrations.local.json"), true, true);
        PreparedBuilder.Configuration.AddEnvironmentVariables();

        PreparedBuilder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });
    }
    
    [Test]
    public async Task Auto_seed_admin_tenant_succeed()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = PreparedBuilder.Configuration.GetConnectionString("MetaConnection");
        
        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });
        
        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = true;
        
        PreparedBuilder.Services.AddBallwareMetaStorage(storageOptions, connectionString);
        
        var app = PreparedBuilder.Build();

        await app.StartAsync();

        using var scope = app.Services.CreateScope();
        
        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();
        var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();

        var adminTenantId = await seeder.GetAdminTenantIdAsync();
        
        Assert.Multiple(() =>
        {
            Assert.That(adminTenantId, Is.Not.Null);
            Assert.That(context.Tenants.FirstOrDefault(t => t.Uuid == adminTenantId), Is.Not.Null);
        });
        
        await app.StopAsync();
    }
    
    [Test]
    public async Task Seed_admin_tenant_not_existing_succeed()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = PreparedBuilder.Configuration.GetConnectionString("MetaConnection");
        
        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });
        
        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = false;
        
        PreparedBuilder.Services.AddBallwareMetaStorage(storageOptions, connectionString);
        
        var app = PreparedBuilder.Build();

        await app.StartAsync();

        using var scope = app.Services.CreateScope();
        
        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();
        var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();

        var expectedAdminTenantId = await seeder.GetAdminTenantIdAsync();

        Assert.That(expectedAdminTenantId, Is.Not.Null);
        
        var existingTenant = context.Tenants.FirstOrDefault(t => t.Uuid == expectedAdminTenantId);

        if (existingTenant != null)
        {
            context.Tenants.Remove(existingTenant);
            await context.SaveChangesAsync();
        }

        var actualTenantId = await seeder.SeedAdminTenantAsync();
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantId, Is.EqualTo(expectedAdminTenantId));
            Assert.That(context.Tenants.FirstOrDefault(t => t.Uuid == expectedAdminTenantId), Is.Not.Null);
        });
        
        await app.StopAsync();
    }
    
    [Test]
    public async Task Seed_admin_tenant_already_existing_succeed()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = PreparedBuilder.Configuration.GetConnectionString("MetaConnection");
        
        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });
        
        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = true;
        
        PreparedBuilder.Services.AddBallwareMetaStorage(storageOptions, connectionString);
        
        var app = PreparedBuilder.Build();

        await app.StartAsync();

        using var scope = app.Services.CreateScope();
        
        var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();
        var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();

        var expectedAdminTenantId = await seeder.GetAdminTenantIdAsync();

        Assert.That(expectedAdminTenantId, Is.Not.Null);

        var actualTenantId = await seeder.SeedAdminTenantAsync();
        
        Assert.Multiple(() =>
        {
            Assert.That(actualTenantId, Is.EqualTo(expectedAdminTenantId));
            Assert.That(context.Tenants.FirstOrDefault(t => t.Uuid == expectedAdminTenantId), Is.Not.Null);
        });
        
        await app.StopAsync();
    }
}