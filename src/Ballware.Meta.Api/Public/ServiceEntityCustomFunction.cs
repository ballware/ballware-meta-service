using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ballware.Meta.Api.Public;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServiceEntityCustomFunctionTypes
{
    [EnumMember(Value="add")]
    Add,
    [EnumMember(Value="edit")]
    Edit,
    [EnumMember(Value="default_add")]
    DefaultAdd,
    [EnumMember(Value="default_view")]
    DefaultView,
    [EnumMember(Value="default_edit")]
    DefaultEdit,
    [EnumMember(Value="external")]
    External,
    [EnumMember(Value="export")]
    Export,
    [EnumMember(Value="import")]
    Import
}

public class ServiceEntityCustomFunction
{
    [JsonPropertyName("id")]
    public required string Identifier { get; set; }
    
    [JsonPropertyName("type")]
    public ServiceEntityCustomFunctionTypes Type { get; set; }
    
    [JsonPropertyName("options")]
    public ServiceEntityCustomFunctionOptions? Options { get; set; }
}

public class ServiceEntityCustomFunctionOptions
{
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("delimiter")]
    public string? Delimiter { get; set; }
}