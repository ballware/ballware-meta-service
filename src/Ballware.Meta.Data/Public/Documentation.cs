namespace Ballware.Meta.Data.Public;

public class Documentation : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public string? Field { get; set; }
    public string? Content { get; set; }
}