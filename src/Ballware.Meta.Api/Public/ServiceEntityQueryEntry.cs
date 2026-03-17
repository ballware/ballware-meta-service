using System.Text.Json.Serialization;

namespace Ballware.Meta.Api.Public;

public class ServiceEntityQueryParameter
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class ServiceEntityQueryResultColumn
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class ServiceEntityQueryEntry
{
    [JsonPropertyName("identifier")]
    public required string Identifier { get; set; }

    [JsonPropertyName("query")]
    public required string Query { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("ai_enabled")]
    public bool? AiEnabled { get; set; }
    
    [JsonPropertyName("parameters")]
    public ServiceEntityQueryParameter[]? Parameters { get; set; }
    
    [JsonPropertyName("result_columns")]
    public ServiceEntityQueryResultColumn[]? ResultColumns { get; set; }
}