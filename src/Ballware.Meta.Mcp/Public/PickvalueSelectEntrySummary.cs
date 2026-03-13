using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PickvalueSelectEntrySummary
{
    [Description("The unique id of this pickvalue entry")]
    public Guid Id { get; set; }

    [Description("The display text of this pickvalue entry")]
    public string? Name { get; set; }

    [Description("The numeric value of this pickvalue entry")]
    public int Value { get; set; }
}
