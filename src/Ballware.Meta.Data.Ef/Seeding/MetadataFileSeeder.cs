using System.Collections.Immutable;
using System.Text.Json;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Document = Ballware.Meta.Data.Public.Document;
using Documentation = Ballware.Meta.Data.Public.Documentation;
using EntityMetadata = Ballware.Meta.Data.Public.EntityMetadata;
using Export = Ballware.Meta.Data.Public.Export;
using Job = Ballware.Meta.Data.Public.Job;
using Lookup = Ballware.Meta.Data.Public.Lookup;
using MlModel = Ballware.Meta.Data.Public.MlModel;
using Notification = Ballware.Meta.Data.Public.Notification;
using NotificationTrigger = Ballware.Meta.Data.Public.NotificationTrigger;
using Page = Ballware.Meta.Data.Public.Page;
using Statistic = Ballware.Meta.Data.Public.Statistic;
using Subscription = Ballware.Meta.Data.Public.Subscription;
using Tenant = Ballware.Meta.Data.Public.Tenant;

namespace Ballware.Meta.Data.Ef.Seeding;

public class MetadataFileSeeder : IMetadataSeeder
{
    private IServiceProvider Services { get; }
    private string? SeedPath { get; }

    private FileStream ReadSeedFile(string filename)
    {
        if (string.IsNullOrEmpty(SeedPath))
        {
            throw new ArgumentException($"SeedPath not defined");
        }
        
        var absolutePath = Path.IsPathRooted(SeedPath);

        var seedFile = absolutePath ? Path.Combine(SeedPath, filename) : Path.Combine(AppContext.BaseDirectory, SeedPath, filename);
        
        if (!File.Exists(seedFile))
        {
            throw new ArgumentException($"SeedPath or file doesn't exist: {seedFile}");
        }

        return new FileStream(seedFile, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private FileStream? ReadOptionalSeedFile(string filename)
    {
        if (string.IsNullOrEmpty(SeedPath))
        {
            throw new ArgumentException($"SeedPath not defined");
        }
        
        var absolutePath = Path.IsPathRooted(SeedPath);

        var seedFile = absolutePath ? Path.Combine(SeedPath, filename) : Path.Combine(AppContext.BaseDirectory, SeedPath, filename);
        
        if (!File.Exists(seedFile))
        {
            return null;
        }

        return new FileStream(seedFile, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public MetadataFileSeeder(IServiceProvider services, string? seedPath)
    {
        Services = services;
        SeedPath = seedPath;
    }
    
    public async Task<Guid?> GetAdminTenantIdAsync()
    {
        var fileStream = ReadSeedFile("admin-tenant.json");

        using var textReader = new StreamReader(fileStream);

        var tenants = JsonSerializer.Deserialize<IEnumerable<Tenant>>(await textReader.ReadToEndAsync());
        var tenant = tenants?.FirstOrDefault();

        return tenant?.Id;
    }

    public async Task<Guid?> SeedAdminTenantAsync(Tenant? tenant = null)
    {
        await using var fileStream = ReadSeedFile("admin-tenant.json");
        using var textReader = new StreamReader(fileStream);

        var tenants = JsonSerializer.Deserialize<IEnumerable<Tenant>>(await textReader.ReadToEndAsync());
        var tenantSeed = tenants?.FirstOrDefault();

        if (tenantSeed != null)
        {
            if (tenant == null)
            {
                tenant = tenantSeed;
            }
            
            tenant.Navigation ??= tenantSeed.Navigation;
            tenant.ReportSchemaDefinition ??= tenantSeed.ReportSchemaDefinition;
            tenant.ServerScriptDefinitions ??= tenantSeed.ServerScriptDefinitions;
            tenant.Templates ??= tenantSeed.Templates;
            tenant.ProviderModelDefinition ??= tenantSeed.ProviderModelDefinition;
            
            var tenantId = tenant.Id;
            
            await Services.GetRequiredService<ITenantableRepository<Tenant>>().SaveAsync(tenantId,null, "seed", ImmutableDictionary<string, object>.Empty, tenant);

            await GenericSeedAsync<Document>(tenantId, "admin-document.json");
            await GenericSeedAsync<Documentation>(tenantId, "admin-documentation.json");
            await GenericSeedAsync<EntityMetadata>(tenantId, "admin-entity.json");
            await GenericSeedAsync<Export>(tenantId, "admin-export.json");
            await GenericSeedAsync<Job>(tenantId, "admin-job.json");
            await GenericSeedAsync<Lookup>(tenantId, "admin-lookup.json");
            await GenericSeedAsync<MlModel>(tenantId, "admin-mlmodel.json");
            await GenericSeedAsync<Notification>(tenantId, "admin-notification.json");
            await GenericSeedAsync<NotificationTrigger>(tenantId, "admin-notificationtrigger.json");
            await GenericSeedAsync<Page>(tenantId, "admin-page.json");
            await GenericSeedAsync<Statistic>(tenantId, "admin-statistic.json");
            await GenericSeedAsync<Subscription>(tenantId, "admin-subscription.json");
        }

        return tenant?.Id;
    }

    public async Task SeedCustomerTenantAsync(Tenant tenant)
    {
        await using var fileStream = ReadSeedFile("customer-tenant.json");
        using var textReader = new StreamReader(fileStream);

        var tenantId = tenant.Id;
        var tenants = JsonSerializer.Deserialize<IEnumerable<Tenant>>(await textReader.ReadToEndAsync());
        var tenantSeed = tenants?.FirstOrDefault();

        if (tenantSeed != null)
        {
            tenant.Navigation ??= tenantSeed.Navigation;
            tenant.ReportSchemaDefinition ??= tenantSeed.ReportSchemaDefinition;
            tenant.ServerScriptDefinitions ??= tenantSeed.ServerScriptDefinitions;
            tenant.Templates ??= tenantSeed.Templates;
            tenant.ProviderModelDefinition ??= tenantSeed.ProviderModelDefinition;

            await Services.GetRequiredService<ITenantableRepository<Tenant>>().SaveAsync(tenantId, null, "seed", ImmutableDictionary<string, object>.Empty, tenant);
        }

        await GenericSeedAsync<Document>(tenantId, "customer-document.json");
        await GenericSeedAsync<Documentation>(tenantId, "customer-documentation.json");
        await GenericSeedAsync<EntityMetadata>(tenantId, "customer-entity.json");
        await GenericSeedAsync<Export>(tenantId, "customer-export.json");
        await GenericSeedAsync<Job>(tenantId, "customer-job.json");
        await GenericSeedAsync<Lookup>(tenantId, "customer-lookup.json");
        await GenericSeedAsync<MlModel>(tenantId, "customer-mlmodel.json");
        await GenericSeedAsync<Notification>(tenantId, "customer-notification.json");
        await GenericSeedAsync<NotificationTrigger>(tenantId, "customer-notificationtrigger.json");
        await GenericSeedAsync<Page>(tenantId, "customer-page.json");
        await GenericSeedAsync<Statistic>(tenantId, "customer-statistic.json");
        await GenericSeedAsync<Subscription>(tenantId, "customer-subscription.json");
    }

    private async Task GenericSeedAsync<TEntity>(Guid tenantId, string filename) where TEntity : class
    {
        await using var fileStream = ReadOptionalSeedFile(filename);

        if (fileStream == null)
        {
            return;
        }

        using var textReader = new StreamReader(fileStream);

        var items = JsonSerializer.Deserialize<IEnumerable<TEntity>>(await textReader.ReadToEndAsync());

        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            await Services.GetRequiredService<ITenantableRepository<TEntity>>().SaveAsync(tenantId, null, "importjson", ImmutableDictionary<string, object>.Empty, item);
        }
    }
}