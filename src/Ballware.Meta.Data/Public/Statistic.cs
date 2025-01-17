namespace Ballware.Meta.Data.Public;

public class Statistic : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public string? Identifier { get; set; }
    public string? Name { get; set; }
    public string? MappingScript { get; set; }
    public string? CustomScripts { get; set; }

    public string? FetchSql { get; set; }
    public string? FetchScript { get; set; }

    public string? Layout { get; set; }
    public bool Meta { get; set; }
}