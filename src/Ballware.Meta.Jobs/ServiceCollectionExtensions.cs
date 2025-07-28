using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Jobs.Internal;
using Ballware.Shared.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;

namespace Ballware.Meta.Jobs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareMetaBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            string importJobName = "import";
            
            q.AddJob<MetaImportJob<Tenant, IRepository<Tenant>>>(new JobKey(importJobName, "tenant"), configurator => configurator.StoreDurably());
                
            q.AddJob<TenantableMetaImportJob<Documentation, ITenantableRepository<Documentation>>>(new JobKey(importJobName, "documentation"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Document, ITenantableRepository<Document>>>(new JobKey(importJobName, "document"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<EntityMetadata, ITenantableRepository<EntityMetadata>>>(new JobKey(importJobName, "entity"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Lookup, ITenantableRepository<Lookup>>>(new JobKey(importJobName, "lookup"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<MlModel, ITenantableRepository<MlModel>>>(new JobKey(importJobName, "mlmodel"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Notification, ITenantableRepository<Notification>>>(new JobKey(importJobName, "notification"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Page, ITenantableRepository<Page>>>(new JobKey(importJobName, "page"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Statistic, ITenantableRepository<Statistic>>>(new JobKey(importJobName, "statistic"), configurator => configurator.StoreDurably());
            q.AddJob<TenantableMetaImportJob<Subscription, ITenantableRepository<Subscription>>>(new JobKey(importJobName, "subscription"), configurator => configurator.StoreDurably());
        });

        services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}