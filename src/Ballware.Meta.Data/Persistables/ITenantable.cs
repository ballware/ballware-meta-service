namespace Ballware.Meta.Data.Persistables;

public interface ITenantable
{
    Guid TenantId { get; set; }
}