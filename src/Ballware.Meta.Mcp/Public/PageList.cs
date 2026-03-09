using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PageList
{
    [Description("Contains the list of pages for this tenant")]
    public required List<PageSummary> Pages { get; set; }
}