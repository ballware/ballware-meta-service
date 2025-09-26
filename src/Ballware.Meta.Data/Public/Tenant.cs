using System.Text.Json;
using Ballware.Shared.Authorization;
using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class ReportDatasourceTable
{
    public string? Name { get; set; }

    public string? Entity { get; set; }

    public string? Query { get; set; }
}

public class ReportDatasourceDefinition
{
    public required string Provider { get; set; }
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }
    public IEnumerable<ReportDatasourceTable> Tables { get; set; } = [];
}

public class Tenant : IEditable, ITenantAuthorizationMetadata
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Navigation { get; set; }
    public virtual string? RightsCheckScript { get; set; }
    public string? Templates { get; set; }

    public string? ServerScriptDefinitions { get; set; }

    public bool ManagedDatabase { get; set; }

    public string? Provider { get; set; }

    public string? Server { get; set; }

    public string? Catalog { get; set; }

    public string? Schema { get; set; }

    public string? User { get; set; }

    public string? Password { get; set; }
    public string? ReportSchemaDefinition { get; set; }
    public string? ProviderModelDefinition { get; set; }
    
    public bool Seed { get; set; }
}

public static class TenantExtensions
{
    public static IEnumerable<ReportDatasourceDefinition>? ToReportSchemaDefinition(this string serializedSchemaDefinition)
    {
        return JsonSerializer.Deserialize<List<ReportDatasourceDefinition>>(serializedSchemaDefinition);
    }
}