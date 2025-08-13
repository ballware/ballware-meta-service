using Ballware.Shared.Data.Repository;

namespace Ballware.Meta.Data.Repository;

public interface IExportMetaRepository : ITenantableRepository<Public.Export>
{
    Task<Public.Export?> ByIdAsync(Guid id);
}