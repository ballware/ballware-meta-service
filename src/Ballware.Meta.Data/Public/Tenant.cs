using System.ComponentModel;
using System.Text.Json;
using Ballware.Shared.Authorization;
using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class ReportDatasourceTable
{
    public string? Name { get; set; }

    public string? Entity { get; set; }

    public string? Query { get; set; }
}

public class ReportDatasourceDefinition
{
    public required string Provider { get; set; }
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }
    public IEnumerable<ReportDatasourceTable> Tables { get; set; } = [];
}

public enum NavigationLayoutItemType
{
    [Description("A link to a page")]
    Page,
    
    [Description("A named group with nexted items")]
    Group,
    
    [Description("A section inside of a group with nexted items")]
    Section
}

public class NavigationLayoutItemOptions
{
    [Description("Page identifier of integrated if this item is of type page")]
    public string? Page { get; set; }
    
    [Description("Url of extenal page if this item is of type page and links to an external page")]
    public string? Url { get; set; }
    
    [Description("Caption to show for this item in the navigation if this item is of type page or group")]
    public string? Caption { get; set; }
    
    [Description("Optional icon to show for this item in the navigation if this item is of type page or group")]
    public string? Icon { get; set; }
}

public class NavigationLayoutItem
{
    [Description("The type of this navigation item")]
    public NavigationLayoutItemType Type { get; set; }
    
    [Description("Optional options for this navigation item depending on the type")]
    public NavigationLayoutItemOptions? Options { get; set; }
    
    [Description("Optional child items if this item is of type group or section")]
    public List<NavigationLayoutItem>? Items { get; set; }
}

public class NavigationLayout
{
    [Description("Headline to show in title of navigation for this tenant")]
    public required string Title { get; set; }
    
    [Description("Optional default page url to show when opening the application for this tenant")]
    public string? DefaultUrl { get; set; }
    
    [Description("The list of navigation items for this tenant")]
    public List<NavigationLayoutItem> Items { get; set; } = [];
}


public class Tenant : IEditable, ITenantAuthorizationMetadata
{
    [Description("The unique identifier for this tenant")]
    public Guid Id { get; set; }
    
    [Description("The name of this tenant")]
    public string? Name { get; set; }
    
    [Description("Embedded JSON schema definition for the tenant's navigation structure")]
    public string? Navigation { get; set; }
    
    [Description("Embedded Javascript implementation of the tenant's rights checking logic")]
    public virtual string? RightsCheckScript { get; set; }
    
    [Description("Embedded JSON schema definition for the tenant's multi purpose edit layout templates")]
    public string? Templates { get; set; }

    [Description("Embedded JSON schema definition for the tenant's server side script definitions, which can be used to implement custom data access logic and be referenced from the multi purpose edit layouts")]
    public string? ServerScriptDefinitions { get; set; }

    [Description("Indicates whether the tenant's database is managed by the system or if connection details are provided by the tenant itself")]
    public bool ManagedDatabase { get; set; }

    [Description("The database provider to use for this tenant, e.g. 'mssql', 'postgres'.")]
    public string? Provider { get; set; }

    [Description("The database server address for this tenant, e.g. 'localhost' or 'db.example.com'.")]
    public string? Server { get; set; }

    [Description("The database name for this tenant.")]
    public string? Catalog { get; set; }

    [Description("The database schema for this tenant, if applicable.")]
    public string? Schema { get; set; }

    [Description("The username for the tenant's database connection.")]
    public string? User { get; set; }

    [Description("The password for the tenant's database connection.")]
    public string? Password { get; set; }
    
    [Description("Embedded JSON schema definition for the tenant's report datasources, which can be used to implement custom reports")]
    public string? ReportSchemaDefinition { get; set; }
    
    [Description("Embedded JSON schema definition for the tenant's custom data provider models, which can be used to define custom database specific objects like views or procedures")]
    public string? ProviderModelDefinition { get; set; }
    
    [Description("Indicates whether this tenant should be seeded when saving changes to the database.")]
    public bool Seed { get; set; }
}

public static class TenantExtensions
{
    public static IEnumerable<ReportDatasourceDefinition>? ToReportSchemaDefinition(this string serializedSchemaDefinition)
    {
        return JsonSerializer.Deserialize<List<ReportDatasourceDefinition>>(serializedSchemaDefinition);
    }
}