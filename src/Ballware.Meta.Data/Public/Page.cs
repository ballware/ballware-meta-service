using System.ComponentModel;
using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class Page : IEditable
{
    [Description("The unique identifier for this tenant")]
    public Guid Id { get; set; }

    [Description("The identifier of this page used for referencing this page in the application")]
    public string? Identifier { get; set; }
    
    [Description("The name of this page to show in the application")]
    public string? Name { get; set; }
    
    [Description("Embedded JSON definition of the layout of this page")]
    public string? Layout { get; set; }
    
    [Description("Embedded JSON definition of the lookups used in page toolbar")]
    public string? Lookups { get; set; }
    
    [Description("Embedded JSON definition of the picklists used in page toolbar")]
    public string? Picklists { get; set; }
    
    [Description("Embedded JavaScript definition of the custom scripts used in page toolbar")]
    public string? CustomScripts { get; set; }
}