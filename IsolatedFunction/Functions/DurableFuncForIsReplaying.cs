using Microsoft.DurableTask;

namespace IsolatedFunction.Functions;

public sealed class DurableFuncForIsReplaying
{
    private readonly ILogger _logger;

    public DurableFuncForIsReplaying(ILogger<DurableFuncForIsReplaying> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Orchestrator_DurableFuncForIsReplaying))]
    public async Task<IEnumerable<string>> Orchestrator_DurableFuncForIsReplaying(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        if (!context.IsReplaying)
            _logger.LogInformation("Retrieve data from the DB.");

        string[] cities = await context.CallActivityAsync<string[]>(nameof(Activity_GetCities));

        IEnumerable<string> outputs = [];

        // Note: The value of IsReplaying will become false when the CallActivityAsync is invoked and the activity commences.
        if (!context.IsReplaying)
            _logger.LogInformation("Start to say hello to the cities.");

        var tasks = new List<Task<string>>();

        foreach (string city in cities)
            tasks.Add(context.CallActivityAsync<string>(nameof(Activity_DurableFuncForIsReplaying), city));

        outputs = await Task.WhenAll(tasks);

        return outputs;
    }

    [Function(nameof(Activity_DurableFuncForIsReplaying))]
    public string Activity_DurableFuncForIsReplaying([ActivityTrigger] string name)
    {
        _logger.LogInformation("Saying hello to {Name}.", name);

        return $"Hello {name}!";
    }

    [Function(nameof(Activity_GetCities))]
    public  string[] Activity_GetCities([ActivityTrigger] object input)
    {
        _logger.LogInformation("Here you can have a DB call to retrieve data for the Orchestrator.");

        return ["Tokyo", "Seattle", "London"];
    }
}
