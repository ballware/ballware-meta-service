﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

[Table("CharacteristicGroup")]
public class CharacteristicGroup : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }

    public virtual string? Entity { get; set; }
    public virtual string? Name { get; set; }
    public virtual int? Sorting { get; set; }
    public virtual string? RegisterName { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}