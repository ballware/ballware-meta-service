using System.Collections.Immutable;
using System.Text.Json;
using Ballware.Shared.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Documentation = Ballware.Meta.Data.Public.Documentation;
using EntityMetadata = Ballware.Meta.Data.Public.EntityMetadata;
using Export = Ballware.Meta.Data.Public.Export;
using Job = Ballware.Meta.Data.Public.Job;
using Lookup = Ballware.Meta.Data.Public.Lookup;
using Page = Ballware.Meta.Data.Public.Page;
using Statistic = Ballware.Meta.Data.Public.Statistic;
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

            await GenericSeedAsync<Documentation>(tenantId, null,"admin-documentation.json");
            await GenericSeedAsync<EntityMetadata>(tenantId, null, "admin-entity.json");
            await GenericSeedAsync<Export>(tenantId, null, "admin-export.json");
            await GenericSeedAsync<Job>(tenantId, null, "admin-job.json");
            await GenericSeedAsync<Lookup>(tenantId, null, "admin-lookup.json");
            await GenericSeedAsync<Page>(tenantId, null, "admin-page.json");
            await GenericSeedAsync<Statistic>(tenantId, null, "admin-statistic.json");
        }

        return tenant?.Id;
    }

    public async Task SeedCustomerTenantAsync(Tenant tenant, Guid userId)
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

            await Services.GetRequiredService<ITenantableRepository<Tenant>>().SaveAsync(tenantId, userId, "seed", ImmutableDictionary<string, object>.Empty, tenant);
        }

        await GenericSeedAsync<Documentation>(tenantId, userId, "customer-documentation.json");
        await GenericSeedAsync<EntityMetadata>(tenantId, userId, "customer-entity.json");
        await GenericSeedAsync<Export>(tenantId, userId, "customer-export.json");
        await GenericSeedAsync<Job>(tenantId, userId, "customer-job.json");
        await GenericSeedAsync<Lookup>(tenantId, userId, "customer-lookup.json");
        await GenericSeedAsync<Page>(tenantId, userId, "customer-page.json");
        await GenericSeedAsync<Statistic>(tenantId, userId, "customer-statistic.json");
    }

    private async Task GenericSeedAsync<TEntity>(Guid tenantId, Guid? userId, string filename) where TEntity : class
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
            await Services.GetRequiredService<ITenantableRepository<TEntity>>().SaveAsync(tenantId, userId, "importjson", ImmutableDictionary<string, object>.Empty, item);
        }
    }
}