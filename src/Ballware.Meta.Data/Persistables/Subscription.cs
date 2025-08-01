using System.Text.Json.Serialization;
using Ballware.Shared.Data.Persistables;

namespace Ballware.Meta.Data.Persistables;

public class Subscription : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonPropertyName(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    public virtual Guid TenantId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string? Mail { get; set; }
    public virtual string? Body { get; set; }
    public virtual bool Attachment { get; set; }
    public virtual string? AttachmentFileName { get; set; }
    public virtual Guid NotificationId { get; set; }
    public virtual int Frequency { get; set; }
    public virtual bool Active { get; set; }
    public virtual DateTime? LastSendStamp { get; set; }
    public virtual string? LastError { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}