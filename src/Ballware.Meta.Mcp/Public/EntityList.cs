using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class EntityList
{
    [Description("Contains the list of entities for this tenant")]
    public required List<EntitySummary> Entities { get; set; }
}
