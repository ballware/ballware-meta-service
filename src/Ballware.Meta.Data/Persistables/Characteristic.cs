using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Ballware.Meta.Data.Common;
using Ballware.Shared.Data.Persistables;

namespace Ballware.Meta.Data.Persistables;

public interface ICharacteristicData
{
    string? Identifier { get; set; }
    string? Name { get; set; }
    EntityCharacteristicTypes Type { get; set; }
    bool? Multi { get; set; }
    Guid? LookupId { get; set; }
    string? LookupValueMember { get; set; }
    string? LookupDisplayMember { get; set; }
}

[Table("Characteristic")]
public class Characteristic : ICharacteristicData, IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }

    public virtual string? Identifier { get; set; }
    public virtual string? Name { get; set; }
    public virtual EntityCharacteristicTypes Type { get; set; }
    public virtual bool? Multi { get; set; }
    public virtual Guid? LookupId { get; set; }
    public virtual string? LookupValueMember { get; set; }
    public virtual string? LookupDisplayMember { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}