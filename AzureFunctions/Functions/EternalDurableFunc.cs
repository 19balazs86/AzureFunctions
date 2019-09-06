using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
  public static class EternalDurableFunc
  {
    [FunctionName(nameof(Client_StartEternalDurableFunc))]
    public static async Task<HttpResponseMessage> Client_StartEternalDurableFunc(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage request,
      [OrchestrationClient] DurableOrchestrationClient starter)
    {
      string instanceId = await starter.StartNewAsync(nameof(Orchestrator_EternalFunc), 1);

      return starter.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(nameof(Orchestrator_EternalFunc))]
    public static async Task<int> Orchestrator_EternalFunc(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log)
    {
      int counter = context.GetInput<int>();

      await context.CallActivityAsync(nameof(Activity_EternalFunc), counter);

      DateTime timeoutAt = context.CurrentUtcDateTime.AddSeconds(counter * 5);

      await context.CreateTimer(timeoutAt, CancellationToken.None);

      if (++counter < 5)
        context.ContinueAsNew(counter);
      else
        log.LogInformation("EternalFunc is stopped.");

      return counter;
    }

    [FunctionName(nameof(Activity_EternalFunc))]
    public static void Activity_EternalFunc([ActivityTrigger] int counter, ILogger log)
      => log.LogInformation($"Counter: {counter}.");
  }
}