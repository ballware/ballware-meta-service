namespace Ballware.Meta.Service.Dtos;

public class MetaTenantDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Navigation { get; set; }
    public string? RightsCheckScript { get; set; }
    public string? Templates { get; set; }
}