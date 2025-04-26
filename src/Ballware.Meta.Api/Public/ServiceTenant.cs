namespace Ballware.Meta.Api.Public;

public class ServiceTenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? RightsCheckScript { get; set; }

    public string? ServerScriptDefinitions { get; set; }

    public bool ManagedDatabase { get; set; }

    public required string Provider { get; set; }

    public string? Server { get; set; }

    public string? Catalog { get; set; }

    public string? Schema { get; set; }

    public string? User { get; set; }

    public string? Password { get; set; }

    public IEnumerable<ServiceTenantReportDatasourceDefinition> ReportDatasourceDefinitions { get; set; }
}