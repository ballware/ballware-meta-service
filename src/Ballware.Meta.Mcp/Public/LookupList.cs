using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class LookupList
{
    [Description("Contains the list of lookups for this tenant")]
    public required List<LookupSummary> Lookups { get; set; }
}
