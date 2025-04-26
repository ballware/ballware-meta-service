using Newtonsoft.Json;

namespace Ballware.Meta.Api.Public;

public class ServiceEntityQueryEntry
{
    [JsonProperty("identifier")]
    public string? Identifier { get; set; }

    [JsonProperty("query")]
    public string? Query { get; set; }
}