using System.Text.Json.Serialization;

namespace Ballware.Meta.Api.Public;

public class ServiceEntityQueryEntry
{
    [JsonPropertyName("identifier")]
    public string? Identifier { get; set; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }
}