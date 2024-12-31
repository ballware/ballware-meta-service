namespace Ballware.Meta.Data.SelectLists;

public class ProcessingStateSelectListEntry
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int State { get; set; }

    public bool Locked { get; set; }
    public bool Finished { get; set; }

    public bool ReasonRequired { get; set; }
}