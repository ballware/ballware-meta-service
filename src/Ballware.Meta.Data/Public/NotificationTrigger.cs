using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class NotificationTrigger : IEditable
{
    public Guid Id { get; set; }

    public Guid NotificationId { get; set; }
    public string? Params { get; set; }
    public bool Finished { get; set; }
}