using Ballware.Meta.Data.Persistables;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef;

public class MetaDbContext : DbContext
{
    public MetaDbContext(DbContextOptions<MetaDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<EntityMetadata> Entities { get; set; }
    public DbSet<EntityRight> EntityRights { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Lookup> Lookups { get; set; }
    public DbSet<Pickvalue> Pickvalues { get; set; }
    public DbSet<ProcessingState> ProcessingStates { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationTrigger> NotificationTriggers { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<CharacteristicGroup> CharacteristicGroups { get; set; }
    public DbSet<Characteristic> Characteristics { get; set; }
    public DbSet<CharacteristicAssociation> CharacteristicAssociations { get; set; }
    public DbSet<Documentation> Documentations { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Export> Exports { get; set; }
    public DbSet<MlModel> MlModels { get; set; }
    public DbSet<TenantDatabaseObject> TenantDatabaseObjects { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>().HasKey(d => d.Id);
        modelBuilder.Entity<Tenant>().HasIndex(d => d.Uuid).IsUnique();
        modelBuilder.Entity<Tenant>().HasIndex(d => d.Name).IsUnique();

        modelBuilder.Entity<TenantDatabaseObject>().HasKey(d => d.Id);
        modelBuilder.Entity<TenantDatabaseObject>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<TenantDatabaseObject>().HasIndex(d => new { d.TenantId, d.Type, d.Name }).IsUnique();

        modelBuilder.Entity<EntityMetadata>().HasKey(d => d.Id);
        modelBuilder.Entity<EntityMetadata>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<EntityMetadata>().HasIndex(d => d.TenantId);

        modelBuilder.Entity<Document>().HasKey(d => d.Id);
        modelBuilder.Entity<Document>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Document>().HasIndex(d => d.TenantId);

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

        modelBuilder.Entity<Notification>().HasKey(d => d.Id);
        modelBuilder.Entity<Notification>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Notification>().HasIndex(d => d.TenantId);
        modelBuilder.Entity<Notification>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();

        modelBuilder.Entity<NotificationTrigger>().HasKey(d => d.Id);
        modelBuilder.Entity<NotificationTrigger>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<NotificationTrigger>().HasIndex(d => d.TenantId);

        modelBuilder.Entity<Subscription>().HasKey(d => d.Id);
        modelBuilder.Entity<Subscription>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Subscription>().HasIndex(d => d.TenantId);
        modelBuilder.Entity<Subscription>().HasIndex(d => d.Frequency);

        modelBuilder.Entity<EntityRight>().HasKey(d => d.Id);
        modelBuilder.Entity<EntityRight>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<EntityRight>().HasIndex(d => new { d.TenantId, d.Entity });

        modelBuilder.Entity<CharacteristicGroup>().HasKey(d => d.Id);
        modelBuilder.Entity<CharacteristicGroup>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<CharacteristicGroup>().HasIndex(d => new { d.TenantId, d.Entity });
        modelBuilder.Entity<CharacteristicGroup>().HasIndex(d => new { d.TenantId, d.Entity, d.Name }).IsUnique();

        modelBuilder.Entity<Characteristic>().HasKey(d => d.Id);
        modelBuilder.Entity<Characteristic>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Characteristic>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Characteristic>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();

        modelBuilder.Entity<CharacteristicAssociation>().HasKey(d => d.Id);
        modelBuilder.Entity<CharacteristicAssociation>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<CharacteristicAssociation>().HasIndex(d => new { d.TenantId, d.Entity });
        modelBuilder.Entity<CharacteristicAssociation>().HasIndex(d => new { d.TenantId, d.Entity, d.CharacteristicId }).IsUnique();

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
        
        modelBuilder.Entity<MlModel>().HasKey(d => d.Id);
        modelBuilder.Entity<MlModel>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<MlModel>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<MlModel>().HasIndex(d => new { d.TenantId, d.Identifier }).IsUnique();
        
        modelBuilder.Entity<Job>().HasKey(d => d.Id);
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId });
        modelBuilder.Entity<Job>().HasIndex(d => new { d.TenantId, d.Owner });

        modelBuilder.Entity<Export>().HasKey(d => d.Id);
        modelBuilder.Entity<Export>().HasIndex(d => new { d.TenantId, d.Uuid }).IsUnique();
        modelBuilder.Entity<Export>().HasIndex(d => new { d.TenantId });
    }
}