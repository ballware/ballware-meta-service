namespace Ballware.Meta.Api.Public;

public class MetaEntity
{
    public Guid Id { get; set; }

    public bool Meta { get; set; }

    public string? Application { get; set; }
    public string? Entity { get; set; }
    public string? DisplayName { get; set; }
    public string? BaseUrl { get; set; }
    public string? ItemMappingScript { get; set; }
    public string? ItemReverseMappingScript { get; set; }

    public string? Lookups { get; set; }
    public string? Picklists { get; set; }
    public string? CustomScripts { get; set; }
    public string? GridLayout { get; set; }
    public string? EditLayout { get; set; }
    public string? CustomFunctions { get; set; }
    public string? StateColumn { get; set; }
    public string? StateReasonColumn { get; set; }
    public string? Templates { get; set; }
}