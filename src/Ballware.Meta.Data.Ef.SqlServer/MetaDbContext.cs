using Ballware.Meta.Data.Persistables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ballware.Meta.Data.Ef.SqlServer;

public class MetaDbContext : DbContext, IMetaDbContext
{
    private ILoggerFactory LoggerFactory { get; }
    
    public MetaDbContext(DbContextOptions<MetaDbContext> options, ILoggerFactory loggerFactory) : base(options)
    {
        LoggerFactory = loggerFactory;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<EntityMetadata> Entities { get; set; }
    public DbSet<EntityRight> EntityRights { get; set; }
    public DbSet<Lookup> Lookups { get; set; }
    public DbSet<Pickvalue> Pickvalues { get; set; }
    public DbSet<ProcessingState> ProcessingStates { get; set; }
    public DbSet<CharacteristicGroup> CharacteristicGroups { get; set; }
    public DbSet<Characteristic> Characteristics { get; set; }
    public DbSet<CharacteristicAssociation> CharacteristicAssociations { get; set; }
    public DbSet<Documentation> Documentations { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Export> Exports { get; set; }
    public DbSet<TenantDatabaseObject> TenantDatabaseObjects { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(LoggerFactory);
        
        base.OnConfiguring(optionsBuilder);
    }
}