using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class EntityRight : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    public string? Container { get; set; }
}
