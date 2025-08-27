using Ballware.Meta.Data.Caching;
using Ballware.Shared.Data.Repository;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Ef.Repository;
using Ballware.Meta.Data.Ef.Seeding;
using Ballware.Meta.Data.Ef.SqlServer.Internal;
using Ballware.Meta.Data.Ef.SqlServer.Model;
using Ballware.Meta.Data.Ef.SqlServer.Repository;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Data.Ef.SqlServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareMetaStorageForSqlServer(this IServiceCollection services, StorageOptions options, string connectionString)
    {
        services.AddSingleton(options);
        services.AddDbContext<MetaDbContext>(o =>
        {
            o.UseSqlServer(connectionString, o =>
            {
                o.MigrationsAssembly(typeof(MetaDbContext).Assembly.FullName);
            });

            o.ReplaceService<IModelCustomizer, SqlServerMetaModelCustomizer>();
        });

        services.AddScoped<IMetaDbContext, MetaDbContext>();
        
        services.AddScoped<ITenantableRepository<Documentation>, DocumentationRepository>();
        services.AddScoped<IDocumentationMetaRepository, DocumentationRepository>();
        
        services.AddScoped<EntityRepository>();
        
        if (options.EnableCaching)
        {
            services.AddScoped<ITenantableRepository<EntityMetadata>, CachableEntityRepository<EntityRepository>>();
            services.AddScoped<IEntityMetaRepository, CachableEntityRepository<EntityRepository>>();
        }
        else
        {
            services.AddScoped<ITenantableRepository<EntityMetadata>, EntityRepository>();
            services.AddScoped<IEntityMetaRepository, EntityRepository>();
        }

        services.AddScoped<ITenantableRepository<Export>, ExportBaseRepository>();
        services.AddScoped<IExportMetaRepository, ExportBaseRepository>();

        services.AddScoped<ITenantableRepository<Job>, JobBaseRepository>();
        services.AddScoped<IJobMetaRepository, JobBaseRepository>();

        services.AddScoped<LookupRepository>();       
        
        if (options.EnableCaching)
        {
            services.AddScoped<ITenantableRepository<Lookup>, CachableLookupRepository<LookupRepository>>();
            services.AddScoped<ILookupMetaRepository, CachableLookupRepository<LookupRepository>>();
        }
        else
        {
            services.AddScoped<ITenantableRepository<Lookup>, LookupRepository>();
            services.AddScoped<ILookupMetaRepository, LookupRepository>();
        }

        services.AddScoped<ITenantableRepository<Page>, PageRepository>();
        services.AddScoped<IPageMetaRepository, PageRepository>();

        services.AddScoped<ITenantableRepository<Pickvalue>, PickvalueRepository>();
        services.AddScoped<IPickvalueMetaRepository, PickvalueRepository>();

        services.AddScoped<ITenantableRepository<ProcessingState>, ProcessingStateRepository>();
        services.AddScoped<IProcessingStateMetaRepository, ProcessingStateRepository>();
        
        services.AddScoped<ITenantableRepository<EntityRight>, EntityRightBaseRepository>();
        services.AddScoped<IEntityRightMetaRepository, EntityRightBaseRepository>();
        
        services.AddScoped<ITenantableRepository<CharacteristicAssociation>, CharacteristicAssociationBaseRepository>();
        services.AddScoped<ICharacteristicAssociationMetaRepository, CharacteristicAssociationBaseRepository>();

        services.AddScoped<ITenantableRepository<Statistic>, StatisticRepository>();
        services.AddScoped<IStatisticMetaRepository, StatisticRepository>();

        services.AddScoped<TenantRepository>();
        
        if (options.EnableCaching)
        {
            services.AddScoped<ITenantableRepository<Tenant>, CachableTenantRepository<TenantRepository>>();
            services.AddScoped<ITenantMetaRepository, CachableTenantRepository<TenantRepository>>();
        }
        else
        {
            services.AddScoped<ITenantableRepository<Tenant>, TenantRepository>();
            services.AddScoped<ITenantMetaRepository, TenantRepository>();
        }
        
        services.AddScoped<IMetadataSeeder>(sp => new MetadataFileSeeder(sp, options.SeedPath));

        services.AddSingleton<IMetaDbConnectionFactory>(new MetaDbConnectionFactory(connectionString));
        services.AddHostedService<InitializationWorker>();

        return services;
    }
}