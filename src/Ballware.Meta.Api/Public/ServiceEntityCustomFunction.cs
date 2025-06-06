using System.Text.Json.Serialization;
using Ballware.Meta.Data.Public;

namespace Ballware.Meta.Api.Public;

public class ServiceEntityCustomFunction
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("type")]
    public EntityCustomFunctionTypes Type { get; set; }
    
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