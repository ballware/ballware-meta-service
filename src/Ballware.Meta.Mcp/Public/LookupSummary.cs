using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class LookupSummary
{
    [Description("The unique id for this lookup")]
    public Guid Id { get; set; }

    [Description("The identifier of this lookup used for referencing this lookup in the application")]
    public required string Identifier { get; set; }

    [Description("The display name of this lookup")]
    public string? Name { get; set; }

    [Description("Flag if this lookup requires parameters")]
    public bool HasParam { get; set; }
}
