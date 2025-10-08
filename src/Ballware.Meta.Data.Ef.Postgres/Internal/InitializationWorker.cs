using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Postgres.Internal;

class InitializationWorker : IHostedService
{
    private IServiceProvider ServiceProvider { get; }

    public InitializationWorker(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var options = scope.ServiceProvider.GetRequiredService<StorageOptions>();

        if (options.AutoMigrations)
        {
            var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();

            await context.Database.MigrateAsync(cancellationToken);
        }
        
        if (options.AutoSeedAdminTenant)
        {
            var repository = scope.ServiceProvider.GetRequiredService<ITenantMetaRepository>();
            var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();

            var adminTenantId = await seeder.GetAdminTenantIdAsync();

            if (adminTenantId != null)
            {
                var existingTenant = await repository.ByIdAsync(adminTenantId.Value);

                if (existingTenant == null)
                {
                    await seeder.SeedAdminTenantAsync();
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
