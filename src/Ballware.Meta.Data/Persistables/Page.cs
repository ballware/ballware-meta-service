using System.Text.Json.Serialization;
using Ballware.Shared.Data.Persistables;

namespace Ballware.Meta.Data.Persistables;

public class Page : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    public virtual string? Identifier { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? Layout { get; set; }
    public virtual string? Lookups { get; set; }
    public virtual string? Picklists { get; set; }
    public virtual string? CustomScripts { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}