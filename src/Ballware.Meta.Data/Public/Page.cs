using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class Page : IEditable
{
    public Guid Id { get; set; }

    public string? Identifier { get; set; }
    public string? Name { get; set; }
    public string? Layout { get; set; }
    public string? Lookups { get; set; }
    public string? Picklists { get; set; }
    public string? CustomScripts { get; set; }
}