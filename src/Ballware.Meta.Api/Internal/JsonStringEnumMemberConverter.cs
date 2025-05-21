using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Ballware.Meta.Api.Internal;

public class JsonStringEnumMemberConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(EnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class EnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly Dictionary<T, string> _enumToString;
        private readonly Dictionary<string, T> _stringToEnum;

        [SuppressMessage("Sonar Code Smell", "S1144:Unused private types or members should be removed", Justification = "Used via reflection")]
        public EnumConverter()
        {
            _enumToString = new Dictionary<T, string>();
            _stringToEnum = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                var member = typeof(T).GetMember(enumValue.ToString())[0];
                var enumMemberAttr = member.GetCustomAttribute<EnumMemberAttribute>();
                var name = enumMemberAttr?.Value ?? enumValue.ToString();

                _enumToString[enumValue] = name;
                _stringToEnum[name] = enumValue;
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str != null && _stringToEnum.TryGetValue(str, out var enumValue))
                return enumValue;

            throw new JsonException($"Unknown value '{str}' for enum type {typeof(T)}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (_enumToString.TryGetValue(value, out var str))
            {
                writer.WriteStringValue(str);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}