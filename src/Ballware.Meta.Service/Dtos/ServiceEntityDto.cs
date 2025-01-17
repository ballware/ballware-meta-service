namespace Ballware.Meta.Service.Dtos;

public class ServiceEntityDto
{
    public Guid Id { get; set; }
    
    public bool GeneratedSchema { get; set; }
    public bool NoIdentity { get; set; }
    
    public string? ListQuery { get; set; }
    public string? ByIdQuery { get; set; }
    public string? NewQuery { get; set; }
    public string? ScalarValueQuery { get; set; }
    public string? SaveStatement { get; set; }
    public string? RemoveStatement { get; set; }
    public string? RemovePreliminaryCheckScript { get; set; }
    public string? ListScript { get; set; }
    public string? RemoveScript { get; set; }
    public string? ByIdScript { get; set; }
    public string? BeforeSaveScript { get; set; }
    public string? SaveScript { get; set; }
    public string? StateAllowedScript { get; set; }
    public string? Indices { get; set; }
}