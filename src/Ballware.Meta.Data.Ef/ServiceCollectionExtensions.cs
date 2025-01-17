using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.Internal;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareMetaStorage(this IServiceCollection services, StorageOptions options, string connectionString)
    {
        services.AddSingleton(options);
        services.AddDbContext<MetaDbContext>(o =>
        {
            o.UseSqlServer(connectionString, o =>
            {
                o.MigrationsAssembly(typeof(MetaDbContext).Assembly.FullName);
            });

        });

        services.AddScoped<ITenantableRepository<Documentation>, DocumentationMetaRepository>();
        services.AddScoped<IDocumentationMetaRepository, DocumentationMetaRepository>();

        services.AddScoped<ITenantableRepository<Document>, DocumentMetaRepository>();
        services.AddScoped<IDocumentMetaRepository, DocumentMetaRepository>();

        services.AddScoped<ITenantableRepository<EntityMetadata>, EntityMetaRepository>();
        services.AddScoped<IEntityMetaRepository, EntityMetaRepository>();

        services.AddScoped<ITenantableRepository<Export>, ExportMetaRepository>();
        services.AddScoped<IExportMetaRepository, ExportMetaRepository>();

        services.AddScoped<ITenantableRepository<Job>, JobMetaRepository>();
        services.AddScoped<IJobMetaRepository, JobMetaRepository>();

        services.AddScoped<ITenantableRepository<Lookup>, LookupMetaRepository>();
        services.AddScoped<ILookupMetaRepository, LookupMetaRepository>();

        services.AddScoped<ITenantableRepository<MlModel>, MlModelMetaRepository>();
        services.AddScoped<IMlModelMetaRepository, MlModelMetaRepository>();

        services.AddScoped<ITenantableRepository<Notification>, NotificationMetaRepository>();
        services.AddScoped<INotificationMetaRepository, NotificationMetaRepository>();

        services.AddScoped<ITenantableRepository<NotificationTrigger>, NotificationTriggerMetaRepository>();
        services.AddScoped<INotificationTriggerMetaRepository, NotificationTriggerMetaRepository>();

        services.AddScoped<ITenantableRepository<Page>, PageMetaRepository>();
        services.AddScoped<IPageMetaRepository, PageMetaRepository>();

        services.AddScoped<ITenantableRepository<Pickvalue>, PickvalueMetaRepository>();
        services.AddScoped<IPickvalueMetaRepository, PickvalueMetaRepository>();

        services.AddScoped<ITenantableRepository<ProcessingState>, ProcessingStateMetaRepository>();
        services.AddScoped<IProcessingStateMetaRepository, ProcessingStateMetaRepository>();

        services.AddScoped<ITenantableRepository<Statistic>, StatisticMetaRepository>();
        services.AddScoped<IStatisticMetaRepository, StatisticMetaRepository>();

        services.AddScoped<ITenantableRepository<Subscription>, SubscriptionMetaRepository>();
        services.AddScoped<ISubscriptionMetaRepository, SubscriptionMetaRepository>();

        services.AddScoped<IRepository<Tenant>, TenantMetaRepository>();
        services.AddScoped<ITenantMetaRepository, TenantMetaRepository>();

        services.AddScoped<IMetadataSeeder>(sp => new MetadataFileSeeder(sp, options.SeedPath));

        services.AddSingleton<IMetaDbConnectionFactory>(new MetaDbConnectionFactory(connectionString));
        services.AddHostedService<InitializationWorker>();

        return services;
    }
}