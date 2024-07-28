using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace IsolatedFunction.Functions;

public sealed class EternalDurableFunc
{
    private readonly ILogger _logger;

    public EternalDurableFunc(ILogger<EternalDurableFunc> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Client_StartEternalDurableFunc))]
    public async Task<HttpResponseData> Client_StartEternalDurableFunc(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request,
        [DurableClient] DurableTaskClient taskClient)
    {
        int counter = 1;

        string instanceId = await taskClient.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator_EternalFunc), counter);

        return taskClient.CreateCheckStatusResponse(request, instanceId);
    }

    [Function(nameof(Orchestrator_EternalFunc))]
    public async Task<int> Orchestrator_EternalFunc(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        int counter)
    {
        await context.CallActivityAsync(nameof(Activity_EternalFunc), counter);

        DateTime timeoutAt = context.CurrentUtcDateTime.AddSeconds(counter * 5);

        await context.CreateTimer(timeoutAt, CancellationToken.None);

        if (++counter < 5)
            context.ContinueAsNew(counter);
        else
            _logger.LogInformation("EternalFunc is stopped.");

        return counter;
    }

    [Function(nameof(Activity_EternalFunc))]
    public void Activity_EternalFunc([ActivityTrigger] int counter)
    {
        _logger.LogInformation("Counter: {Counter}.", counter);
    }
}
