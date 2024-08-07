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
    public static async Task<string[]> Orchestrator_DurableFuncForIsReplaying(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger replaySafeLogger = context.CreateReplaySafeLogger<DurableFuncForIsReplaying>();

        replaySafeLogger.LogInformation("Retrieve data from the DB."); // if (!context.IsReplaying) _logger.LogInformation("Retrieve data from the DB.");

        string[] cities = await context.CallActivityAsync<string[]>(nameof(Activity_GetCities));

        replaySafeLogger.LogInformation("Start to say hello to the cities.");

        List<Task<string>> tasks = [];

        foreach (string city in cities)
        {
            tasks.Add(context.CallActivityAsync<string>(nameof(Activity_DurableFuncForIsReplaying), city));
        }

        return await Task.WhenAll(tasks);
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

        return ["Sub-Tokyo", "Sub-Seattle", "Sub-London"];
    }
}
