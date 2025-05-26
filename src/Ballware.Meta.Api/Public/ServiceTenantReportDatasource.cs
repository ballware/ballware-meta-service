namespace Ballware.Meta.Api.Public;

public class ServiceTenantReportDatasourceTable
{
    public string? Name { get; set; }

    public string? Entity { get; set; }

    public string? Query { get; set; }
}

public class ServiceTenantReportDatasourceDefinition
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public IEnumerable<ServiceTenantReportDatasourceTable>? Tables { get; set; }
}