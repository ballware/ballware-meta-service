using System.Net.Sockets;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Api.Public;

[JsonConverter(typeof(StringEnumConverter))]
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
    [JsonProperty("id")]
    public string Identifier { get; set; }
    
    [JsonProperty("type")]
    public ServiceEntityCustomFunctionTypes Type { get; set; }
    
    [JsonProperty("options")]
    public ServiceEntityCustomFunctionOptionsDto? Options { get; set; }
}

public class ServiceEntityCustomFunctionOptionsDto
{
    [JsonProperty("format")]
    public string? Format { get; set; }

    [JsonProperty("delimiter")]
    public string? Delimiter { get; set; }
}