using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
  public static class DurableFuncForIsReplaying
  {
    [FunctionName(nameof(DurableFuncForIsReplaying_Orchestrator))]
    public static async Task<IEnumerable<string>> DurableFuncForIsReplaying_Orchestrator(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log)
    {
      var outputs = new List<string>();

      if (!context.IsReplaying)
        log.LogInformation("Call say hello to Tokyo.");

      outputs.Add(await context.CallActivityAsync<string>(nameof(DurableFuncForIsReplaying_Activity), "Tokyo"));

      // NOTE: IsReplaying became false IF the CallActivityAsync is called AND the activity kicks off.
      if (!context.IsReplaying)
        log.LogInformation("Call say hello to Seattle.");

      outputs.Add(await context.CallActivityAsync<string>(nameof(DurableFuncForIsReplaying_Activity), "Seattle"));

      if (!context.IsReplaying)
        log.LogInformation("Call say hello to London.");

      outputs.Add(await context.CallActivityAsync<string>(nameof(DurableFuncForIsReplaying_Activity), "London"));

      return outputs;
    }

    [FunctionName(nameof(DurableFuncForIsReplaying_Activity))]
    public static string DurableFuncForIsReplaying_Activity([ActivityTrigger] string name, ILogger log)
    {
      log.LogInformation($"Saying hello to {name}.");

      return $"Hello {name}!";
    }
  }
}