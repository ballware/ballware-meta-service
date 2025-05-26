namespace Ballware.Meta.Data.SelectLists;

public class EntityRightSelectListEntry
{
    public Guid Id { get; set; }
    public required string Identifier { get; set; }
    public string? Name { get; set; }
    public string? Container { get; set; }
}