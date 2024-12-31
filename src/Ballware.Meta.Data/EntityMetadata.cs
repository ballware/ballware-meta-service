using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data;

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

[Table("Entity")]
public class EntityMetadata : IEntity, IAuditable, ITenantable
{
    [JsonIgnore]
    public virtual long? Id { get; set; }

    [JsonProperty(nameof(Id))]
    public virtual Guid Uuid { get; set; }

    [JsonIgnore]
    public virtual Guid TenantId { get; set; }
    public virtual bool Meta { get; set; }

    [JsonIgnore]
    public virtual bool GeneratedSchema { get; set; }

    [JsonIgnore]
    public virtual bool NoIdentity { get; set; }
    public virtual string? Application { get; set; }
    public virtual string? Entity { get; set; }
    public virtual string? DisplayName { get; set; }
    public virtual string? BaseUrl { get; set; }
    public virtual string? ItemMappingScript { get; set; }
    public virtual string? ItemReverseMappingScript { get; set; }

    [JsonIgnore]
    public virtual string? ListQuery { get; set; }

    [JsonIgnore]
    public virtual string? ByIdQuery { get; set; }

    [JsonIgnore]
    public virtual string? NewQuery { get; set; }

    [JsonIgnore]
    public virtual string? ScalarValueQuery { get; set; }

    [JsonIgnore]
    public virtual string? SaveStatement { get; set; }

    [JsonIgnore]
    public virtual string? RemoveStatement { get; set; }

    [JsonIgnore]
    public virtual string? RemovePreliminaryCheckScript { get; set; }

    [JsonIgnore]
    public virtual string? ListScript { get; set; }
    
    [JsonIgnore]
    public virtual string? RemoveScript { get; set; }

    [JsonIgnore]
    public virtual string? ByIdScript { get; set; }

    [JsonIgnore]
    public virtual string? BeforeSaveScript { get; set; }

    [JsonIgnore]
    public virtual string? SaveScript { get; set; }
    public virtual string? Lookups { get; set; }
    public virtual string? Picklists { get; set; }
    public virtual string? CustomScripts { get; set; }
    public virtual string? GridLayout { get; set; }
    public virtual string? EditLayout { get; set; }
    public virtual string? CustomFunctions { get; set; }
    public virtual string? StateColumn { get; set; }
    public virtual string? StateReasonColumn { get; set; }
    public virtual string? Templates { get; set; }

    [JsonIgnore]
    public virtual string? StateAllowedScript { get; set; }

    [JsonIgnore]
    public virtual string? Indices { get; set; }

    [JsonIgnore]
    public virtual Guid? CreatorId { get; set; }

    [JsonIgnore]
    public virtual DateTime? CreateStamp { get; set; }

    [JsonIgnore]
    public virtual Guid? LastChangerId { get; set; }

    [JsonIgnore]
    public virtual DateTime? LastChangeStamp { get; set; }
}