using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.Internal;
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

        services.AddScoped<IDocumentationMetaRepository, DocumentationMetaRepository>();
        services.AddScoped<IDocumentMetaRepository, DocumentMetaRepository>();
        services.AddScoped<IEntityMetaRepository, EntityMetaRepository>();
        services.AddScoped<IExportMetaRepository, ExportMetaRepository>();
        services.AddScoped<IJobMetaRepository, JobMetaRepository>();
        services.AddScoped<ILookupMetaRepository, LookupMetaRepository>();
        services.AddScoped<IMlModelMetaRepository, MlModelMetaRepository>();
        services.AddScoped<INotificationMetaRepository, NotificationMetaRepository>();
        services.AddScoped<INotificationTriggerMetaRepository, NotificationTriggerMetaRepository>();
        services.AddScoped<IPageMetaRepository, PageMetaRepository>();
        services.AddScoped<IPickvalueMetaRepository, PickvalueMetaRepository>();
        services.AddScoped<IProcessingStateMetaRepository, ProcessingStateMetaRepository>();
        services.AddScoped<IStatisticMetaRepository, StatisticMetaRepository>();
        services.AddScoped<ISubscriptionMetaRepository, SubscriptionMetaRepository>();
        services.AddScoped<ITenantMetaRepository, TenantMetaRepository>();
        
        services.AddSingleton<IMetaDbConnectionFactory>(new MetaDbConnectionFactory(connectionString));
        services.AddHostedService<InitializationWorker>();
        
        return services;
    }
}