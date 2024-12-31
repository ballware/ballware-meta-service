using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

public enum DatabaseObjectTypes
{
    Unknown = 0,
    Table = 1,
    View = 2,
    Function = 3,
    Type = 4,
    Statement = 5
}

[Table("TenantDatabaseObject")]
public class TenantDatabaseObject : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }

    public virtual string? Name { get; set; }

    public virtual DatabaseObjectTypes Type { get; set; }

    [JsonIgnore]
    public virtual string? Sql { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}