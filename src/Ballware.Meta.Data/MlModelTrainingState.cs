namespace Ballware.Meta.Data;

public class MlModelTrainingState
{
    public Guid Id { get; set; }
    public MlModelTrainingStates State { get; set; }
    public string? Result { get; set; }
}