using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Data;

[JsonConverter(typeof(StringEnumConverter))]
public enum MlModelTypes : int
{
    Undefined = 0,
    Regression = 1
}    