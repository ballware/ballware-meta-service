using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Data.Common;

[JsonConverter(typeof(StringEnumConverter))]
public enum JobStates
{
    Unknown = 0,
    Queued = 1,
    InProgress = 5,
    Finished = 10,
    Error = 99
}