using System.Text.Json.Serialization;

namespace Ballware.Meta.Data.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobStates
{
    Unknown = 0,
    Queued = 1,
    InProgress = 5,
    Finished = 10,
    Error = 99
}