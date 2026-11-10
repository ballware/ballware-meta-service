using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PickvalueAvailabilityList
{
    [Description("Contains the list of available entity/field combinations with pickvalues for this tenant")]
    public required List<PickvalueAvailabilitySummary> Availabilities { get; set; }
}
