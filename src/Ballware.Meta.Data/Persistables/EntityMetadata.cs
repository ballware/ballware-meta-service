using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Ballware.Shared.Data.Persistables;

namespace Ballware.Meta.Data.Persistables;

[Table("Entity")]
public class EntityMetadata : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public Guid Uuid { get; set; }

    [JsonIgnore]
    public Guid TenantId { get; set; }
    public bool Meta { get; set; }

    public bool GeneratedSchema { get; set; }

    public bool NoIdentity { get; set; }
    public string? Application { get; set; }
    public string? Entity { get; set; }
    public string? DisplayName { get; set; }
    public string? BaseUrl { get; set; }
    public string? ItemMappingScript { get; set; }
    public string? ItemReverseMappingScript { get; set; }

    public string? ListQuery { get; set; }

    public string? ByIdQuery { get; set; }

    public string? NewQuery { get; set; }

    public string? ScalarValueQuery { get; set; }

    public string? SaveStatement { get; set; }

    public string? RemoveStatement { get; set; }

    public string? RemovePreliminaryCheckScript { get; set; }

    public string? ListScript { get; set; }

    public string? RemoveScript { get; set; }

    public string? ByIdScript { get; set; }

    public string? BeforeSaveScript { get; set; }

    public string? SaveScript { get; set; }
    public string? Lookups { get; set; }
    public string? Picklists { get; set; }
    public string? CustomScripts { get; set; }
    public string? GridLayout { get; set; }
    public string? EditLayout { get; set; }
    public string? CustomFunctions { get; set; }
    public string? StateColumn { get; set; }
    public string? StateReasonColumn { get; set; }
    public string? Templates { get; set; }

    public string? StateAllowedScript { get; set; }

    public string? Indices { get; set; }
    public string? ProviderModelDefinition { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? CreateStamp { get; set; }

    public Guid? LastChangerId { get; set; }

    public DateTime? LastChangeStamp { get; set; }
}