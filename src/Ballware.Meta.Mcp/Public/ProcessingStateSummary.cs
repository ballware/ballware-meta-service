using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class ProcessingStateSummary
{
    [Description("The unique id of this processing state")]
    public Guid Id { get; set; }

    [Description("The display name of this processing state")]
    public string? Name { get; set; }

    [Description("The numeric state code")]
    public int State { get; set; }

    [Description("Flag indicating whether records in this state are locked")]
    public bool Locked { get; set; }

    [Description("Flag indicating whether this state marks a record as finished")]
    public bool Finished { get; set; }

    [Description("Flag indicating whether a reason is required when entering this state")]
    public bool ReasonRequired { get; set; }
}
