using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.DurableEntities;

public interface ICounter
{
    Task<int> Add(int amount);
    Task<int> Get();
    Task Delete();
}

[JsonObject(MemberSerialization.OptIn)]
public class Counter : ICounter
{
    [FunctionName(nameof(Counter))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx)
    {
        return ctx.DispatchAsync<Counter>();
    }

    private readonly ILogger<Counter> _logger;

    // DI is working by default.
    // Add custom input (not from DI):
    // https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities#bindings-in-entity-classes
    public Counter(ILogger<Counter> logger) => _logger = logger;

    [JsonProperty(nameof(Value))]
    public int Value { get; set; }

    public async Task<int> Add(int amount)
    {
        _logger.LogInformation("Counter.Add -> {value} + {amount}", Value, amount);

        await Task.Delay(1000);

        Value += amount;

        if (Value % 3 == 0)
        {
            _logger.LogInformation("{value} % 3 == true.", Value);

            var input = new CounterOrchestratorInput { EntityKey = Entity.Current.EntityKey, Amount = -1 };

            Entity.Current.StartNewOrchestration(nameof(CounterFunctions.CounterOrchestrator), input);
        }

        return Value;
    }

    public Task<int> Get() => Task.FromResult(Value);

    public Task Delete()
    {
        Entity.Current.DeleteState();

        return Task.CompletedTask;
    }
}
