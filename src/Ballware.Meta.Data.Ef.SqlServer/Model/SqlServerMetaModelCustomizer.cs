using Ballware.Meta.Data.Ef.Model;
using Ballware.Meta.Data.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ballware.Meta.Data.Ef.SqlServer.Model;

public class SqlServerMetaModelCustomizer : MetaModelBaseCustomizer
{
    public SqlServerMetaModelCustomizer(ModelCustomizerDependencies dependencies) 
        : base(dependencies)
    {
    }
    
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);
        
        modelBuilder.Entity<Tenant>().ToTable("Tenant");
        modelBuilder.Entity<TenantDatabaseObject>().ToTable("TenantDatabaseObject");
        modelBuilder.Entity<EntityMetadata>().ToTable("Entity");
        modelBuilder.Entity<Lookup>().ToTable("Lookup");
        modelBuilder.Entity<Pickvalue>().ToTable("Pickvalue");
        modelBuilder.Entity<ProcessingState>().ToTable("ProcessingState");
        modelBuilder.Entity<EntityRight>().ToTable("EntityRight");
        modelBuilder.Entity<CharacteristicGroup>().ToTable("CharacteristicGroup");
        modelBuilder.Entity<Characteristic>().ToTable("Characteristic");
        modelBuilder.Entity<CharacteristicAssociation>().ToTable("CharacteristicAssociation");
        modelBuilder.Entity<Documentation>().ToTable("Documentation");
        modelBuilder.Entity<Statistic>().ToTable("Statistic");
        modelBuilder.Entity<Page>().ToTable("Page");
        modelBuilder.Entity<Job>().ToTable("Job");
        modelBuilder.Entity<Export>().ToTable("Export");
    }
}