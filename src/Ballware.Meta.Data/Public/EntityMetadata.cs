using Newtonsoft.Json;

namespace Ballware.Meta.Data.Public;

public class EntityQueryEntry
{
    [JsonProperty("identifier")]
    public string? Identifier { get; set; }
    
    [JsonProperty("query")]
    public string? Query { get; set; }
}

public class EntityCustomFunction
{
    [JsonProperty("id")]
    public string? Identifier { get; set; }
}

public class EntityImportFunctionOptions
{
    [JsonProperty("format")]
    public string? Format { get; set; }

    [JsonProperty("delimiter")]
    public string? Delimiter { get; set; }
}

public class EntityExportFunctionOptions
{
    [JsonProperty("format")]
    public string? Format { get; set; }

    [JsonProperty("delimiter")]
    public string? Delimiter { get; set; }
}

public class EntityCustomExportFunction : EntityCustomFunction
{
    [JsonProperty("options")]
    public EntityExportFunctionOptions? Options { get; set; }
}

public class EntityCustomImportFunction : EntityCustomFunction
{
    [JsonProperty("options")]
    public EntityImportFunctionOptions? Options { get; set; }
}

public class EntityCustomScripts
{
    [JsonProperty("extendedRightsCheck")]
    public string? ExtendedRightsCheck { get; set; }

    [JsonProperty("rightsParamForHead")]
    public string? RightsParamForHead { get; set; }

    [JsonProperty("rightsParamForItem")]
    public string? RightsParamForItem { get; set; }

    [JsonProperty("prepareCustomParam")]
    public string? PrepareCustomParam { get; set; }

    [JsonProperty("prepareGridLayout")]
    public string? PrepareGridLayout { get; set; }

    [JsonProperty("prepareEditLayout")]
    public string? PrepareEditLayout { get; set; }

    [JsonProperty("editorPreparing")]
    public string? EditorPreparing { get; set; }

    [JsonProperty("editorInitialized")]
    public string? EditorInitialized { get; set; }

    [JsonProperty("editorValueChanged")]
    public string? EditorValueChanged { get; set; }

    [JsonProperty("editorEntered")]
    public string? EditorEntered { get; set; }

    [JsonProperty("editorEvent")]
    public string? EditorEvent { get; set; }

    [JsonProperty("editorValidating")]
    public string? EditorValidating { get; set; }

    [JsonProperty("detailGridCellPreparing")]
    public string? DetailGridCellPreparing { get; set; }

    [JsonProperty("detailGridRowValidating")]
    public string? DetailGridRowValidating { get; set; }

    [JsonProperty("initNewDetailItem")]
    public string? InitNewDetailItem { get; set; }

    [JsonProperty("prepareCustomFunction")]
    public string? PrepareCustomFunction { get; set; }

    [JsonProperty("evaluateCustomFunction")]
    public string? EvaluateCustomFunction { get; set; }

    [JsonProperty("prepareTemplateInstance")]
    public string? PrepareTemplateInstance { get; set; }
}

public class EntityIndex
{
    [JsonProperty("identifier")]
    public string? Identifier { get; set; }

    [JsonProperty("unique")]
    public bool Unique { get; set; }

    [JsonProperty("members")]
    public string[]? Members { get; set; }
}

public static class MetadataExtensions
{
    public static EntityQueryEntry? GetQueryByIdentifier(this string serializedQueryContainer, string identifier)
    {
        var queries = JsonConvert.DeserializeObject<EntityQueryEntry[]>(serializedQueryContainer);

        return queries?.FirstOrDefault(q => identifier.Equals(q.Identifier, StringComparison.OrdinalIgnoreCase));
    }

    public static EntityCustomImportFunction? GetImportFunctionByIdentifier(this string serializedFunctionsContainer, string identifier)
    {
        var queries = JsonConvert.DeserializeObject<EntityCustomImportFunction[]>(serializedFunctionsContainer);

        return queries?.FirstOrDefault(q => identifier.Equals(q.Identifier, StringComparison.OrdinalIgnoreCase));
    }

    public static EntityCustomExportFunction? GetExportFunctionByIdentifier(this string serializedFunctionsContainer, string identifier)
    {
        var queries = JsonConvert.DeserializeObject<EntityCustomExportFunction[]>(serializedFunctionsContainer);

        return queries?.FirstOrDefault(q => identifier.Equals(q.Identifier, StringComparison.OrdinalIgnoreCase));
    }

    public static EntityCustomScripts? GetCustomScripts(this string serializedCustomScripts)
    {
        return JsonConvert.DeserializeObject<EntityCustomScripts>(serializedCustomScripts);
    }

    public static IEnumerable<EntityIndex>? GetEntityIndices(this EntityMetadata metadata)
    {
        return JsonConvert.DeserializeObject<EntityIndex[]>(metadata.Indices ?? "[]");
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
    public string? CustomScripts { get; set; }
    public string? GridLayout { get; set; }
    public string? EditLayout { get; set; }
    public string? CustomFunctions { get; set; }
    public string? StateColumn { get; set; }
    public string? StateReasonColumn { get; set; }
    public string? Templates { get; set; }

    public string? StateAllowedScript { get; set; }

    public string? Indices { get; set; }
    
    public IEnumerable<ProcessingState> States { get; set; } = Array.Empty<ProcessingState>();
    public IEnumerable<EntityRight> Rights { get; set; } = Array.Empty<EntityRight>();
    public IEnumerable<Pickvalue> Pickvalues { get; set; } = Array.Empty<Pickvalue>();
    public IEnumerable<CharacteristicAssociation> CharacteristicAssociations { get; set; } = Array.Empty<CharacteristicAssociation>();
}