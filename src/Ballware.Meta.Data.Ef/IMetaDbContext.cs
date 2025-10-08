using Ballware.Meta.Data.Persistables;
using Ballware.Shared.Data.Ef;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef;

public interface IMetaDbContext : IDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<EntityMetadata> Entities { get; }
    DbSet<EntityRight> EntityRights { get; }
    DbSet<Lookup> Lookups { get; }
    DbSet<Pickvalue> Pickvalues { get; }
    DbSet<ProcessingState> ProcessingStates { get; }
    DbSet<Documentation> Documentations { get; }
    DbSet<Statistic> Statistics { get; }
    DbSet<Page> Pages { get; }
    DbSet<Export> Exports { get; }
    DbSet<Job> Jobs { get; }
}