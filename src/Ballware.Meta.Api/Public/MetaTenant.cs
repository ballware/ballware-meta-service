namespace Ballware.Meta.Api.Public;

public class MetaTenant
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Navigation { get; set; }
    public string? RightsCheckScript { get; set; }
    public string? Templates { get; set; }
}