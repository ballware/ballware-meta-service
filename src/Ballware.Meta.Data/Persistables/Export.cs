using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Persistables;

[Table("Export")]
public class Export : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }

    public virtual string? Application { get; set; }
    public virtual string? Entity { get; set; }
    public virtual string? Query { get; set; }
    public virtual DateTime? ExpirationStamp { get; set; }
    public virtual string? MediaType { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}