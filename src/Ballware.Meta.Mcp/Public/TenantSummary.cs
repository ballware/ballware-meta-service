using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class TenantSummary
{
    [Description("The unique identifier for this tenant")]
    public Guid Id { get; set; }
    
    [Description("The name of this tenant")]
    public string? Name { get; set; }
}