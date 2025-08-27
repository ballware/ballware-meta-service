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
        if (SeedPath == null || !File.Exists(Path.Combine(SeedPath, filename)))
        {
            throw new ArgumentException($"SeedPath or file doesn't exist: {filename}");
        }

        return new FileStream(Path.Combine(SeedPath, filename), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private FileStream? ReadOptionalSeedFile(string filename)
    {
        if (SeedPath == null || !File.Exists(Path.Combine(SeedPath, filename)))
        {
            return null;
        }

        return new FileStream(Path.Combine(SeedPath, filename), FileMode.Open, FileAccess.Read, FileShare.Read);
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

    public async Task<Guid?> SeedAdminTenantAsync()
    {
        await using var fileStream = ReadSeedFile("admin-tenant.json");
        using var textReader = new StreamReader(fileStream);

        var tenants = JsonSerializer.Deserialize<IEnumerable<Tenant>>(await textReader.ReadToEndAsync());
        var tenant = tenants?.FirstOrDefault();

        if (tenant != null)
        {
            await Services.GetRequiredService<ITenantableRepository<Tenant>>().SaveAsync(tenant.Id,null, "seed", ImmutableDictionary<string, object>.Empty, tenant);

            var tenantId = tenant.Id;

            await GenericSeedAsync<Documentation>(tenantId, "admin-documentation.json");
            await GenericSeedAsync<EntityMetadata>(tenantId, "admin-entity.json");
            await GenericSeedAsync<Export>(tenantId, "admin-export.json");
            await GenericSeedAsync<Job>(tenantId, "admin-job.json");
            await GenericSeedAsync<Lookup>(tenantId, "admin-lookup.json");
            await GenericSeedAsync<Page>(tenantId, "admin-page.json");
            await GenericSeedAsync<Statistic>(tenantId, "admin-statistic.json");
        }

        return tenant?.Id;
    }

    public async Task SeedCustomerTenantAsync(Guid tenantId, string name)
    {
        await using var fileStream = ReadSeedFile("customer-tenant.json");
        using var textReader = new StreamReader(fileStream);

        var tenants = JsonSerializer.Deserialize<IEnumerable<Tenant>>(await textReader.ReadToEndAsync());
        var tenant = tenants?.FirstOrDefault();

        if (tenant != null)
        {
            tenant.Id = tenantId;
            tenant.Name = name;

            await Services.GetRequiredService<ITenantableRepository<Tenant>>().SaveAsync(tenantId, null, "seed", ImmutableDictionary<string, object>.Empty, tenant);
        }

        await GenericSeedAsync<Documentation>(tenantId, "customer-documentation.json");
        await GenericSeedAsync<EntityMetadata>(tenantId, "customer-entity.json");
        await GenericSeedAsync<Export>(tenantId, "customer-export.json");
        await GenericSeedAsync<Job>(tenantId, "customer-job.json");
        await GenericSeedAsync<Lookup>(tenantId, "customer-lookup.json");
        await GenericSeedAsync<Page>(tenantId, "customer-page.json");
        await GenericSeedAsync<Statistic>(tenantId, "customer-statistic.json");
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