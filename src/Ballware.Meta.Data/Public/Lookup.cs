using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class Lookup : IEditable
{
    public Guid Id { get; set; }

    public bool Meta { get; set; }
    public bool HasParam { get; set; }
    public int Type { get; set; }

    public string? Identifier { get; set; }
    public string? Name { get; set; }

    public string? ListQuery { get; set; }
    public string? ByIdQuery { get; set; }
}