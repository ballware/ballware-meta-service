using Newtonsoft.Json;

namespace Ballware.Meta.Data.SelectLists;

public class PickvalueSelectEntry
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Value { get; set; }
}
