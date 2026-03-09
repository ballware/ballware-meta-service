using System.Text.Json;
using NJsonSchema.Generation;

namespace Ballware.Meta.Mcp.Endpoints;

internal static class JsonSchemaDefaults
{
    public static readonly JsonSchemaGeneratorSettings SchemaSettings = new SystemTextJsonSchemaGeneratorSettings
    {
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }
    };
}