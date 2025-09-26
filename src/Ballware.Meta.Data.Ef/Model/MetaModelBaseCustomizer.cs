using Ballware.Meta.Data.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ballware.Meta.Data.Ef.Model;

public class MetaModelBaseCustomizer : RelationalModelCustomizer
{
    public MetaModelBaseCustomizer(ModelCustomizerDependencies dependencies) 
        : base(dependencies)
    {
    }
    
    private static void ApplyValueConverter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                        v => v.HasValue ? v.Value.ToUniversalTime() : v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                }
            }
        }
    }
    
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        ApplyValueConverter(modelBuilder);
        
        modelBuilder.Entity<Tenant>().HasKey(d => d.Id);
        modelBuilder.Entity<Tenant>().HasIndex(d => d.Uuid).IsUnique();
        modelBuilder.Entity<Tenant>().HasIndex(d => d.Name).IsUnique();

        modelBuilder.Entity<EntityMetadata>().HasKey(d => d.Id);
        modelBuilder.Entity<EntityMetadata>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<EntityMetadata>().HasIndex(d => d.TenantId);

        modelBuilder.Entity<Lookup>().HasKey(d => d.Id);
        modelBuilder.Entity<Lookup>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Lookup>().HasIndex(d => d.TenantId);
        modelBuilder.Entity<Lookup>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();

        modelBuilder.Entity<Pickvalue>().HasKey(d => d.Id);
        modelBuilder.Entity<Pickvalue>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Pickvalue>().HasIndex(d => new { d.TenantId, d.Entity });

        modelBuilder.Entity<ProcessingState>().HasKey(d => d.Id);
        modelBuilder.Entity<ProcessingState>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<ProcessingState>().HasIndex(d => new { d.TenantId, d.Entity });

        modelBuilder.Entity<EntityRight>().HasKey(d => d.Id);
        modelBuilder.Entity<EntityRight>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<EntityRight>().HasIndex(d => new { d.TenantId, d.Entity });

        modelBuilder.Entity<Documentation>().HasKey(d => d.Id);
        modelBuilder.Entity<Documentation>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Documentation>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Documentation>().HasIndex(d => new { d.TenantId, d.Entity, d.Field }).IsUnique();

        modelBuilder.Entity<Statistic>().HasKey(d => d.Id);
        modelBuilder.Entity<Statistic>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Statistic>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Statistic>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();

        modelBuilder.Entity<Page>().HasKey(d => d.Id);
        modelBuilder.Entity<Page>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Page>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Page>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();

        modelBuilder.Entity<Job>().HasKey(d => d.Id);
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId, d.Owner });

        modelBuilder.Entity<Export>().HasKey(d => d.Id);
        modelBuilder.Entity<Export>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Export>().HasIndex(d => new { d.TenantId });
    }
}