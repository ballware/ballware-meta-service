using System.Text.Json.Serialization;

namespace Ballware.Meta.Data.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MlModelTrainingStates
{
    Unknown = 0,
    Outdated = 1,
    Queued = 5,
    InProgress = 6,
    UpToDate = 10,
    Error = 99
}