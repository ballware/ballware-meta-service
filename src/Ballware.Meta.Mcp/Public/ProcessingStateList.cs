using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class ProcessingStateList
{
    [Description("Contains the list of processing states")]
    public required List<ProcessingStateSummary> ProcessingStates { get; set; }
}
