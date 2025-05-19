using System.Text.Json.Serialization;

namespace Ballware.Meta.Data.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MlModelTypes : int
{
    Undefined = 0,
    Regression = 1
}