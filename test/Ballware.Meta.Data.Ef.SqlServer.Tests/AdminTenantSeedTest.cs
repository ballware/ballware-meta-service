using System.Reflection;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.SqlServer;
using Ballware.Meta.Data.Ef.Tests.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.Tests;

[TestFixture]
public class AdminTenantSeedTest : DatabaseBackedBaseTest
{
    [SetUp]
    public void Setup()
    {
        PreparedBuilder.Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
        });
    }

    [Test]
    public async Task Auto_seed_admin_tenant_succeed()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = MasterConnectionString;

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = true;

        PreparedBuilder.Services.AddBallwareMetaStorageForSqlServer(storageOptions, connectionString);
        
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
        var connectionString = MasterConnectionString;

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = false;

        PreparedBuilder.Services.AddBallwareMetaStorageForSqlServer(storageOptions, connectionString);

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
            Assert.That(context.Entities.Count(e => e.TenantId == actualTenantId), Is.EqualTo(16));
            Assert.That(context.EntityRights.Count(er => er.TenantId == actualTenantId && er.Entity == "entity"), Is.EqualTo(6));
            Assert.That(context.ProcessingStates.Count(er => er.TenantId == actualTenantId && er.Entity == "document"), Is.EqualTo(4));
            Assert.That(context.Pickvalues.Count(er => er.TenantId == actualTenantId && er.Entity == "subscription" && er.Field == "frequency"), Is.EqualTo(2));
        });

        await app.StopAsync();
    }

    [Test]
    public async Task Seed_admin_tenant_already_existing_succeed()
    {
        var storageOptions = PreparedBuilder.Configuration.GetSection("Storage").Get<StorageOptions>();
        var connectionString = MasterConnectionString;

        Assert.Multiple(() =>
        {
            Assert.That(storageOptions, Is.Not.Null);
            Assert.That(connectionString, Is.Not.Null);
        });

        storageOptions.SeedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "seed");
        storageOptions.AutoSeedAdminTenant = true;

        PreparedBuilder.Services.AddBallwareMetaStorageForSqlServer(storageOptions, connectionString);

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