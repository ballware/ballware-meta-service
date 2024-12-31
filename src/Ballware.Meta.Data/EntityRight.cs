using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

[Table("EntityRight")]
public class EntityRight : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public Guid Uuid { get; set; }

    [JsonIgnore]
    public Guid TenantId { get; set; }
    public string? Entity { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    public string? Container { get; set; }

    [JsonIgnore]
    public Guid? CreatorId { get; set; }

    [JsonIgnore]
    public DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public DateTime? LastChangeStamp { get; set; }
}
