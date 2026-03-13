using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class StatisticList
{
    [Description("Contains the list of statistics for this tenant")]
    public required List<StatisticSummary> Statistics { get; set; }
}
