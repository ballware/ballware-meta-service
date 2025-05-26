using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ballware.Meta.Data.Persistables;

[Table("Lookup")]
public class Lookup : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    public virtual bool Meta { get; set; }
    public virtual bool HasParam { get; set; }
    public virtual int Type { get; set; }

    [Required]
    public virtual string? Identifier { get; set; }
    public virtual string? Name { get; set; }

    [JsonIgnore]
    public virtual string? ListQuery { get; set; }

    [JsonIgnore]
    public virtual string? ByIdQuery { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}