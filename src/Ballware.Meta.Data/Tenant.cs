using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

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

[Table("Tenant")]
public class Tenant : IEntity, IAuditable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Navigation { get; set; }
    public virtual string? RightsCheckScript { get; set; }
    public virtual string? Templates { get; set; }

    [JsonIgnore]
    public virtual string? ServerScriptDefinitions { get; set; }

    [JsonIgnore]
    public virtual bool ManagedDatabase { get; set; }

    [JsonIgnore]
    [MaxLength(20)]
    [Required]
    public virtual string Provider { get; set; }
    
    [JsonIgnore]
    public virtual string? Server { get; set; }

    [JsonIgnore]
    public virtual string? Catalog { get; set; }

    [JsonIgnore]
    public virtual string? Schema { get; set; }

    [JsonIgnore]
    public virtual string? User { get; set; }

    [JsonIgnore]
    public virtual string? Password { get; set; }

    [JsonIgnore]
    public virtual string? ReportSchemaDefinition { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
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