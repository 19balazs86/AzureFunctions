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
      if (!context.IsReplaying)
        log.LogInformation("Retrieve data from the DB.");

      string[] cities = await context.CallActivityAsync<string[]>(nameof(GetCities_Activity), null);

      var outputs = new List<string>();

      // NOTE: IsReplaying became false IF the CallActivityAsync is called AND the activity kicks off.
      if (!context.IsReplaying)
        log.LogInformation("Start to say hello to the cities.");

      foreach (string city in cities)
        outputs.Add(await context.CallActivityAsync<string>(nameof(DurableFuncForIsReplaying_Activity), city));

      return outputs;
    }

    [FunctionName(nameof(DurableFuncForIsReplaying_Activity))]
    public static string DurableFuncForIsReplaying_Activity([ActivityTrigger] string name, ILogger log)
    {
      log.LogInformation($"Saying hello to {name}.");

      return $"Hello {name}!";
    }

    [FunctionName(nameof(GetCities_Activity))]
    public static string[] GetCities_Activity([ActivityTrigger] object input, ILogger log)
    {
      log.LogInformation("Here you can have a DB call to retrieve data for the Orchestrator.");

      return new string[] { "Tokyo", "Seattle", "London" };
    }
  }
}