namespace Ballware.Meta.Service.Dtos;

public class NotificationTriggerDto
{
    public Guid Id { get; set; }

    public Guid NotificationId { get; set; }
    public string? Params { get; set; }
    public bool Finished { get; set; }
}