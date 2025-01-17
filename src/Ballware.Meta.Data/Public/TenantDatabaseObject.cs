using Ballware.Meta.Data.Common;

namespace Ballware.Meta.Data.Public;

public class TenantDatabaseObject : IEditable
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DatabaseObjectTypes Type { get; set; }

    public string? Sql { get; set; }
}