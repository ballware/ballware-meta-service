using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class EntitySummary
{
    [Description("The unique id for this entity")]
    public Guid Id { get; set; }
    
    [Description("The identifier of this entity used for referencing this entity in the application")]
    public required string Entity { get; set; }
    
    [Description("The display name of this entity")]
    public string? Name { get; set; }
}
