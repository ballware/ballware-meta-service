using System.ComponentModel.DataAnnotations.Schema;
using Ballware.Meta.Data.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Data.Persistables;

[Table("Job")]
public class Job : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    
    public virtual string? Scheduler { get; set; }
    
    public virtual string? Identifier { get; set; }
    
    public virtual Guid? Owner { get; set; }
    
    public virtual string? Options { get; set; }
    
    public virtual string? Result { get; set; }
    
    public virtual JobStates State { get; set; }
    
    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}
