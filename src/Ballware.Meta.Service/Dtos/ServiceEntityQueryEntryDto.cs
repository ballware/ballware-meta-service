using Newtonsoft.Json;

namespace Ballware.Meta.Service.Dtos;

public class ServiceEntityQueryEntryDto
{
    [JsonProperty("identifier")]
    public string? Identifier { get; set; }

    [JsonProperty("query")]
    public string? Query { get; set; }
}