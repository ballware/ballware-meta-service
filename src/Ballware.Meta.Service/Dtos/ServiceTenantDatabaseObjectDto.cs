using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ballware.Meta.Service.Dtos;

[JsonConverter(typeof(StringEnumConverter))]
public enum ServiceTenantDatabaseObjectTypes
{
    [EnumMember(Value = "unknown")]
    Unknown = 0,
    [EnumMember(Value = "table")]
    Table = 1,
    [EnumMember(Value = "view")]
    View = 2,
    [EnumMember(Value = "function")]
    Function = 3,
    [EnumMember(Value = "type")]
    Type = 4,
    [EnumMember(Value = "statement")]
    Statement = 5
}

public class ServiceTenantDatabaseObjectDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public ServiceTenantDatabaseObjectTypes Type { get; set; }

    public string? Sql { get; set; }
}