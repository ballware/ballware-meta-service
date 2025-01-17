using Ballware.Meta.Data.Common;

namespace Ballware.Meta.Data.Public;

public class Job : IEditable
{
    public Guid Id { get; set; }

    public string? Scheduler { get; set; }

    public string? Identifier { get; set; }

    public Guid? Owner { get; set; }

    public string? Options { get; set; }

    public string? Result { get; set; }

    public JobStates State { get; set; }
}
