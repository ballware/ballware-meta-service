using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Data;

[JsonConverter(typeof(StringEnumConverter))]
public enum MlModelTrainingStates
{
    Unknown = 0,
    Outdated = 1,
    Queued = 5,
    InProgress = 6,
    UpToDate = 10,
    Error = 99
}

[Table("MlModel")]
public class MlModel : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    
    public virtual string? Identifier { get; set; }
    
    public virtual MlModelTypes Type { get; set; }
    
    [JsonIgnore]
    public virtual string? TrainSql { get; set; }
    
    public virtual MlModelTrainingStates TrainState { get; set; }
    
    public virtual string? TrainResult { get; set; }
    
    public virtual string? Options { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}