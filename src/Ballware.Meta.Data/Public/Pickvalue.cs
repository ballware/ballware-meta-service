using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class Pickvalue : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public string? Field { get; set; }
    public int Value { get; set; }
    public string? Text { get; set; }
    public int? Sorting { get; set; }
}

public class PickvalueAvailability
{
    public string? Entity { get; set; }
    public string? Field { get; set; }
}