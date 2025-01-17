using Ballware.Meta.Data.Ef.Configuration;

namespace Ballware.Meta.Data.Ef.Internal;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;

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
            var seeder = scope.ServiceProvider.GetRequiredService<IMetadataSeeder>();

            await seeder.SeedAdminTenantAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
