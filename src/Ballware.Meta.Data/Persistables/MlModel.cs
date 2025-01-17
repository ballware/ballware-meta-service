using System.ComponentModel.DataAnnotations.Schema;
using Ballware.Meta.Data.Common;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Persistables;

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