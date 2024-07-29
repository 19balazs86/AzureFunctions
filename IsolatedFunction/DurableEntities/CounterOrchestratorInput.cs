namespace IsolatedFunction.DurableEntities;

public sealed class CounterOrchestratorInput
{
    public required string EntityKey { get; set; }
    public required int Amount { get; set; }
}
