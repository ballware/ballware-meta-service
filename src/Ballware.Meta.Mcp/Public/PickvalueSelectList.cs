using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PickvalueSelectList
{
    [Description("Contains the list of pickvalue entries for the requested entity/field combination")]
    public required List<PickvalueSelectEntrySummary> Pickvalues { get; set; }
}
