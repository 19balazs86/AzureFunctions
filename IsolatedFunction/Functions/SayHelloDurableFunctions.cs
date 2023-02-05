using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace IsolatedFunction.Functions;

public sealed class SayHelloDurableFunctions
{
    private readonly ILogger _logger;

    public SayHelloDurableFunctions(ILogger<SayHelloDurableFunctions> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Client_SayHello))]
    public async Task<HttpResponseData> Client_SayHello(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request,
        [DurableClient] DurableTaskClient taskClient)
    {
        var sayHelloRequest = await request.ReadFromJsonAsync<SayHelloRequest>();

        //var options = new StartOrchestrationOptions { StartAt = DateTimeOffset.UtcNow.AddSeconds(10) };

        string instanceId = await taskClient.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator_SayHello), sayHelloRequest);

        _logger.LogInformation("Started orchestration with ID = '{InstanceId}'.", instanceId);

        return taskClient.CreateCheckStatusResponse(request, instanceId);
    }

    [Function(nameof(Orchestrator_SayHello))]
    public async Task<IEnumerable<string>> Orchestrator_SayHello(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        SayHelloRequest request)
    {
        _logger.LogInformation($"Run: {nameof(Orchestrator_SayHello)}.");

        // context.CurrentUtcDateTime
        // context.NewGuid()

        var tasks = new List<Task<string>>();

        //foreach (string city in request.CityNames)
        //  tasks.Add(context.CallActivityAsync<string>(nameof(SayHello_Activity), city));

        TaskOptions options = TaskOptions.FromRetryPolicy(new RetryPolicy(3, TimeSpan.FromSeconds(2)));

        foreach (string city in request.CityNames)
            tasks.Add(context.CallActivityAsync<string>(nameof(Activity_SayHello), city, options));

        IEnumerable<string> results = Enumerable.Empty<string>();

        try
        {
            results = await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            if (!context.IsReplaying)
                _logger.LogError(ex, "Error during to call say hello.");
        }

        // --> CallSubOrchestrator
        if (!context.IsReplaying)
            _ = await context.CallSubOrchestratorAsync<IEnumerable<string>>(nameof(DurableFuncForIsReplaying.Orchestrator_DurableFuncForIsReplaying));

        return results;
    }

    [Function(nameof(Activity_SayHello))]
    public async Task<string> Activity_SayHello([ActivityTrigger] string city)
    {
        if (Random.Shared.NextDouble() < 0.05 && city.Contains('a'))
            throw new InvalidProgramException($"Random exception for {city}.");

        _logger.LogInformation("Wait and saying hello to '{City}'.", city);

        await Task.Delay(3000);

        return $"Hello {city}!";
    }
}

public sealed class SayHelloRequest
{
    public IEnumerable<string> CityNames { get; set; }
}