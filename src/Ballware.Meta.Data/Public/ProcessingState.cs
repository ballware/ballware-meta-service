using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class ProcessingState : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public int State { get; set; }
    public string? Name { get; set; }
    public string? Successors { get; set; }
    public bool RecordFinished { get; set; }
    public bool RecordLocked { get; set; }
    public bool ReasonRequired { get; set; }
}