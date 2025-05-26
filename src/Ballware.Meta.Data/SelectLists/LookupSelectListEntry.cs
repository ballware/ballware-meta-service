namespace Ballware.Meta.Data.SelectLists;

public class LookupSelectListEntry
{
    public Guid Id { get; set; }
    public required string Identifier { get; set; }
    public string? Name { get; set; }
    public bool HasParam { get; set; }
}
