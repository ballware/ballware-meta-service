using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ballware.Meta.Data.Public;

public class EntityCustomFunction
{
    [JsonPropertyName("id")]
    public string? Identifier { get; set; }
}

public class EntityImportFunctionOptions
{
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("delimiter")]
    public string? Delimiter { get; set; }
}

public class EntityExportFunctionOptions
{
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("delimiter")]
    public string? Delimiter { get; set; }
}

public class EntityCustomScripts
{
    [JsonPropertyName("extendedRightsCheck")]
    public string? ExtendedRightsCheck { get; set; }

    [JsonPropertyName("rightsParamForHead")]
    public string? RightsParamForHead { get; set; }

    [JsonPropertyName("rightsParamForItem")]
    public string? RightsParamForItem { get; set; }

    [JsonPropertyName("prepareCustomParam")]
    public string? PrepareCustomParam { get; set; }

    [JsonPropertyName("prepareGridLayout")]
    public string? PrepareGridLayout { get; set; }

    [JsonPropertyName("prepareEditLayout")]
    public string? PrepareEditLayout { get; set; }

    [JsonPropertyName("editorPreparing")]
    public string? EditorPreparing { get; set; }

    [JsonPropertyName("editorInitialized")]
    public string? EditorInitialized { get; set; }

    [JsonPropertyName("editorValueChanged")]
    public string? EditorValueChanged { get; set; }

    [JsonPropertyName("editorEntered")]
    public string? EditorEntered { get; set; }

    [JsonPropertyName("editorEvent")]
    public string? EditorEvent { get; set; }

    [JsonPropertyName("editorValidating")]
    public string? EditorValidating { get; set; }

    [JsonPropertyName("detailGridCellPreparing")]
    public string? DetailGridCellPreparing { get; set; }

    [JsonPropertyName("detailGridRowValidating")]
    public string? DetailGridRowValidating { get; set; }

    [JsonPropertyName("initNewDetailItem")]
    public string? InitNewDetailItem { get; set; }

    [JsonPropertyName("prepareCustomFunction")]
    public string? PrepareCustomFunction { get; set; }

    [JsonPropertyName("evaluateCustomFunction")]
    public string? EvaluateCustomFunction { get; set; }

    [JsonPropertyName("prepareTemplateInstance")]
    public string? PrepareTemplateInstance { get; set; }
}

public static class MetadataExtensions
{
    public static EntityCustomScripts? GetCustomScripts(this string serializedCustomScripts)
    {
        return JsonSerializer.Deserialize<EntityCustomScripts>(serializedCustomScripts);
    }
}

public class EntityMetadata : IEditable
{
    public Guid Id { get; set; }

    public bool Meta { get; set; }

    public bool GeneratedSchema { get; set; }

    public bool NoIdentity { get; set; }
    public string? Application { get; set; }
    public string? Entity { get; set; }
    public string? DisplayName { get; set; }
    public string? BaseUrl { get; set; }
    public string? ItemMappingScript { get; set; }
    public string? ItemReverseMappingScript { get; set; }

    public string? ListQuery { get; set; }

    public string? ByIdQuery { get; set; }

    public string? NewQuery { get; set; }

    public string? ScalarValueQuery { get; set; }

    public string? SaveStatement { get; set; }

    public string? RemoveStatement { get; set; }

    public string? RemovePreliminaryCheckScript { get; set; }

    public string? ListScript { get; set; }

    public string? RemoveScript { get; set; }

    public string? ByIdScript { get; set; }

    public string? BeforeSaveScript { get; set; }

    public string? SaveScript { get; set; }
    public string? Lookups { get; set; }
    public string? Picklists { get; set; }
    public virtual string? CustomScripts { get; set; }
    public string? GridLayout { get; set; }
    public string? EditLayout { get; set; }
    public string? CustomFunctions { get; set; }
    public string? StateColumn { get; set; }
    public string? StateReasonColumn { get; set; }
    public string? Templates { get; set; }

    public string? StateAllowedScript { get; set; }

    public string? Indices { get; set; }
    public string? ProviderModelDefinition { get; set; }

    public IEnumerable<ProcessingState> States { get; set; } = Array.Empty<ProcessingState>();
    public IEnumerable<EntityRight> Rights { get; set; } = Array.Empty<EntityRight>();
    public IEnumerable<Pickvalue> Pickvalues { get; set; } = Array.Empty<Pickvalue>();
    public IEnumerable<CharacteristicAssociation> CharacteristicAssociations { get; set; } = Array.Empty<CharacteristicAssociation>();
}