using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class StatisticSummary
{
    [Description("The unique id of this statistic")]
    public Guid Id { get; set; }

    [Description("The identifier of this statistic used for referencing it in the application")]
    public required string Identifier { get; set; }

    [Description("The display name of this statistic")]
    public string? Name { get; set; }
}
