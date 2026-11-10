using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PickvalueAvailabilitySummary
{
    [Description("The entity name this pickvalue is defined for")]
    public string? Entity { get; set; }

    [Description("The field name this pickvalue is defined for")]
    public string? Field { get; set; }
}
