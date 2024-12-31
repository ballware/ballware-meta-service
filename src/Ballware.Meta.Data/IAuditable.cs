namespace Ballware.Meta.Data;

public interface IAuditable {
    Guid? CreatorId { get; set; }
    DateTime? CreateStamp { get; set; }
    Guid? LastChangerId { get; set; }
    DateTime? LastChangeStamp { get; set; }
}

