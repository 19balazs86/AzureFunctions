using Microsoft.DurableTask.Entities;

namespace IsolatedFunction.DurableEntities;

public sealed class CounterState
{
    public int CounterValue { get; set; }
    public int MethodCallCounter { get; set; }
}

public sealed class Counter : TaskEntity<CounterState>
{
    [Function(nameof(Counter))]
    public static Task Run([EntityTrigger] TaskEntityDispatcher ctx)
    {
        return ctx.DispatchAsync<Counter>();
    }

    private readonly ILogger<Counter> _logger;

    // DI is working by default
    // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities?pivots=isolated#dependency-injection-in-entity-classes-1
    public Counter(ILogger<Counter> logger)
    {
        _logger = logger;
    }

    public async Task<int> Add(int amount)
    {
        _logger.LogInformation("Counter.Add -> {value} + {amount}", State.CounterValue, amount);

        await Task.Delay(1000);

        State.MethodCallCounter++;
        State.CounterValue += amount;

        if (State.CounterValue % 3 == 0)
        {
            _logger.LogInformation("{value} % 3 == true.", State.CounterValue);

            var input = new CounterOrchestratorInput { EntityKey = Context.Id.ToString(), Amount = -1 };

            Context.ScheduleNewOrchestration(nameof(CounterFunctions.CounterOrchestrator), input);
        }

        return State.CounterValue;
    }

    public static EntityInstanceId CreateEntityId(string entityKey)
    {
        return new EntityInstanceId(nameof(Counter), entityKey);
    }
}
