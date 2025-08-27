using Ballware.Meta.Data.Ef.Model;
using Ballware.Meta.Data.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ballware.Meta.Data.Ef.Postgres.Model;

public class PostgresMetaModelCustomizer : MetaModelBaseCustomizer
{
    public PostgresMetaModelCustomizer(ModelCustomizerDependencies dependencies) 
        : base(dependencies)
    {
    }
    
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);
        
        modelBuilder.Entity<Tenant>().ToTable("tenant");
        modelBuilder.Entity<TenantDatabaseObject>().ToTable("tenant_database_object");
        modelBuilder.Entity<EntityMetadata>().ToTable("entity");
        modelBuilder.Entity<Lookup>().ToTable("lookup");
        modelBuilder.Entity<Pickvalue>().ToTable("pickvalue");
        modelBuilder.Entity<ProcessingState>().ToTable("processing_state");
        modelBuilder.Entity<EntityRight>().ToTable("entity_right");
        modelBuilder.Entity<CharacteristicGroup>().ToTable("characteristic_group");
        modelBuilder.Entity<Characteristic>().ToTable("characteristic");
        modelBuilder.Entity<CharacteristicAssociation>().ToTable("characteristic_association");
        modelBuilder.Entity<Documentation>().ToTable("documentation");
        modelBuilder.Entity<Statistic>().ToTable("statistic");
        modelBuilder.Entity<Page>().ToTable("page");
        modelBuilder.Entity<Job>().ToTable("job");
        modelBuilder.Entity<Export>().ToTable("export");
    }
}