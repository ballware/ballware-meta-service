namespace Ballware.Meta.Data.SelectLists;

public class EntitySelectListEntry
{
    public Guid Id { get; set; }
    public required string Entity { get; set; }
    public string? Name { get; set; }
}
