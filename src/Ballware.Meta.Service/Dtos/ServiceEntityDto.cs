namespace Ballware.Meta.Service.Dtos;

public class ServiceEntityDto
{
    public Guid Id { get; set; }

    public string? Application { get; set; }
    public string? Entity { get; set; }
    
    public ServiceEntityCustomScriptsDto? CustomScripts { get; set; }
    
    public bool GeneratedSchema { get; set; }
    public bool NoIdentity { get; set; }
    
    public IEnumerable<ServiceEntityCustomFunctionDto> CustomFunctions { get; set; }
    public IEnumerable<ServiceEntityQueryEntryDto> ListQuery { get; set; }
    public IEnumerable<ServiceEntityQueryEntryDto> ByIdQuery { get; set; }
    public IEnumerable<ServiceEntityQueryEntryDto> NewQuery { get; set; }
    public string? ScalarValueQuery { get; set; }
    public IEnumerable<ServiceEntityQueryEntryDto> SaveStatement { get; set; }
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