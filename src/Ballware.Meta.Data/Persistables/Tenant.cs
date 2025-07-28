using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Ballware.Shared.Data.Persistables;

namespace Ballware.Meta.Data.Persistables;

[Table("Tenant")]
public class Tenant : IEntity, IAuditable
{
    [JsonIgnore]
    public long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public Guid Uuid { get; set; }
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
    
    public string? ProviderModelDefinition { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? CreateStamp { get; set; }

    public Guid? LastChangerId { get; set; }

    public DateTime? LastChangeStamp { get; set; }
}