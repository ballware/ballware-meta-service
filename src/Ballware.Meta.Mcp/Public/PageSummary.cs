using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class PageSummary
{
    [Description("The unique id for this page")]
    public Guid Id { get; set; }
    
    [Description("The identifier of this page used for referencing this page in the application")]
    public required string Identifier { get; set; }
    
    [Description("The display name of this page")]
    public string? Name { get; set; }
}