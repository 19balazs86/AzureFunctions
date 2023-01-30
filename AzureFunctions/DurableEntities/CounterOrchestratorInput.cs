namespace AzureFunctions.DurableEntities;

public class CounterOrchestratorInput
{
    public string EntityKey { get; set; }
    public int Amount { get; set; }
}
