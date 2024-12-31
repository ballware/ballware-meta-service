using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

[Table("CharacteristicAssociation")]
public class CharacteristicAssociation : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    public virtual string? Entity { get; set; }
    public virtual string? Identifier { get; set; }
    public virtual EntityCharacteristicTypes Type { get; set; }
    public virtual int? Length { get; set; }
    public virtual Guid? CharacteristicId { get; set; }
    public virtual Guid? CharacteristicGroupId { get; set; }
    public virtual bool? Active { get; set; }
    public virtual bool? Required { get; set; }
    public virtual bool? Readonly { get; set; }
    public virtual int? Sorting { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}
