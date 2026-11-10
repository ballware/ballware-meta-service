using System.ComponentModel;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Mcp.Public;

public class TenantNavigation
{
    [Description("The unique identifier for this tenant")]
    public Guid Id { get; set; }

    [Description("User navigation layout for this tenant")]
    public required NavigationLayout Layout { get; set; }
}