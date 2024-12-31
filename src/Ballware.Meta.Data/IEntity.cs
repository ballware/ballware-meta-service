namespace Ballware.Meta.Data;

public interface IEntity
{
    long? Id { get; set; }
    Guid Uuid { get; set; }
}

