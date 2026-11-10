using System.ComponentModel;

namespace Ballware.Meta.Mcp.Public;

public class StatisticMetadata
{
    [Description("The unique id of this statistic")]
    public Guid Id { get; set; }

    [Description("The entity this statistic is associated with")]
    public string? Entity { get; set; }

    [Description("The identifier of this statistic used for referencing it in the application")]
    public string? Identifier { get; set; }

    [Description("The display name of this statistic")]
    public string? Name { get; set; }

    [Description("Script for mapping fetched data to the statistic layout")]
    public string? MappingScript { get; set; }

    [Description("Custom scripts for extended statistic behavior")]
    public string? CustomScripts { get; set; }

    [Description("SQL query used to fetch statistic data")]
    public string? FetchSql { get; set; }

    [Description("Script used to fetch statistic data dynamically")]
    public string? FetchScript { get; set; }

    [Description("Layout definition for rendering the statistic")]
    public string? Layout { get; set; }

    [Description("Flag indicating whether this is a meta statistic")]
    public bool Meta { get; set; }
}
