namespace Ballware.Meta.Data;

public interface ITenantable
{
    Guid TenantId { get; set; }
}