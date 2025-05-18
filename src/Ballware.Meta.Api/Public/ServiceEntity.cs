namespace Ballware.Meta.Api.Public;

public class ServiceEntity
{
    public Guid Id { get; set; }

    public string? Application { get; set; }
    public string? Entity { get; set; }
    
    public ServiceEntityCustomScripts? CustomScripts { get; set; }
    
    public bool GeneratedSchema { get; set; }
    public bool NoIdentity { get; set; }
    
    public IEnumerable<ServiceEntityCustomFunction> CustomFunctions { get; set; }
    public IEnumerable<ServiceEntityQueryEntry> ListQuery { get; set; }
    public IEnumerable<ServiceEntityQueryEntry> ByIdQuery { get; set; }
    public IEnumerable<ServiceEntityQueryEntry> NewQuery { get; set; }
    public string? ScalarValueQuery { get; set; }
    public string? StateColumn { get; set; }
    public IEnumerable<ServiceEntityQueryEntry> SaveStatement { get; set; }
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