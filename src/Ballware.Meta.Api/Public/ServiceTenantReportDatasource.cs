namespace Ballware.Meta.Api.Public;

public class ServiceTenantReportDatasourceTable
{
    public string? Name { get; set; }

    public string? Entity { get; set; }

    public string? Query { get; set; }

    public IEnumerable<ServiceTenantReportDatasourceRelation>? Relations { get; set; }
}

public class ServiceTenantReportDatasourceRelation
{
    public string? Name { get; set; }
    public string? ChildTable { get; set; }
    public string? MasterColumn { get; set; }
    public string? ChildColumn { get; set; }
}

public class ServiceTenantReportDatasourceDefinition
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public IEnumerable<ServiceTenantReportDatasourceTable>? Tables { get; set; }
}