using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
  public static class DurableFuncForIsReplaying
  {
    [FunctionName(nameof(Orchestrator_DurableFuncForIsReplaying))]
    public static async Task<IEnumerable<string>> Orchestrator_DurableFuncForIsReplaying(
      [OrchestrationTrigger] IDurableOrchestrationContext context,
      ILogger log)
    {
      if (!context.IsReplaying)
        log.LogInformation("Retrieve data from the DB.");

      string[] cities = await context.CallActivityAsync<string[]>(nameof(Activity_GetCities), null);

      var outputs = new List<string>();

      // NOTE: IsReplaying became false IF the CallActivityAsync is called AND the activity kicks off.
      if (!context.IsReplaying)
        log.LogInformation("Start to say hello to the cities.");

      foreach (string city in cities)
        outputs.Add(await context.CallActivityAsync<string>(nameof(Activity_DurableFuncForIsReplaying), city));

      return outputs;
    }

    [FunctionName(nameof(Activity_DurableFuncForIsReplaying))]
    public static string Activity_DurableFuncForIsReplaying([ActivityTrigger] string name, ILogger log)
    {
      log.LogInformation($"Saying hello to {name}.");

      return $"Hello {name}!";
    }

    [FunctionName(nameof(Activity_GetCities))]
    public static string[] Activity_GetCities([ActivityTrigger] object input, ILogger log)
    {
      log.LogInformation("Here you can have a DB call to retrieve data for the Orchestrator.");

      return new string[] { "Tokyo", "Seattle", "London" };
    }
  }
}