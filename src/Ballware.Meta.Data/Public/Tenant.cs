using Newtonsoft.Json;

namespace Ballware.Meta.Data.Public;

public class ReportDatasourceTable
{
    public string? Name { get; set; }

    public string? Entity { get; set; }

    public string? Query { get; set; }

    public IEnumerable<ReportDatasourceRelation>? Relations { get; set; }
}

public class ReportDatasourceRelation
{
    public string? Name { get; set; }
    public string? ChildTable { get; set; }
    public string? MasterColumn { get; set; }
    public string? ChildColumn { get; set; }
}

public class ReportDatasourceDefinition
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public IEnumerable<ReportDatasourceTable>? Tables { get; set; }
}

public class Tenant : IEditable
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Navigation { get; set; }
    public string? RightsCheckScript { get; set; }
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
}

public static class TenantExtensions
{
    public static IEnumerable<ReportDatasourceDefinition>? ToReportSchemaDefinition(this string serializedSchemaDefinition)
    {
        using (var textReader = new StringReader(serializedSchemaDefinition))
        using (var jsonReader = new JsonTextReader(textReader))
        {
            var schemaDefinitions = JsonSerializer.Create().Deserialize<List<ReportDatasourceDefinition>>(jsonReader);

            return schemaDefinitions;
        }
    }
}