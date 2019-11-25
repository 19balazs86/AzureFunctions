using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
  public static class SayHelloDurableFunctions
  {
    private static readonly Random _random = new Random();

    [FunctionName(nameof(Client_SayHello))]
    public static async Task<HttpResponseMessage> Client_SayHello(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage request,
      [DurableClient] IDurableClient starter,
      ILogger log)
    {
      var sayHelloRequest = await request.Content.ReadAsAsync<SayHelloRequest>();

      string instanceId = await starter.StartNewAsync(nameof(Orchestrator_SayHello), sayHelloRequest);

      log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

      return starter.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(nameof(Orchestrator_SayHello))]
    public static async Task<IEnumerable<string>> Orchestrator_SayHello(
      [OrchestrationTrigger] IDurableOrchestrationContext context,
      ILogger log)
    {
      log.LogInformation($"Run: {nameof(Orchestrator_SayHello)}.");

      // context.CurrentUtcDateTime
      // context.NewGuid()

      var request = context.GetInput<SayHelloRequest>();
      var tasks   = new List<Task<string>>();

      //foreach (string city in request.CityNames)
      //  tasks.Add(context.CallActivityAsync<string>(nameof(SayHello_Activity), city));

      var retryOptions = new RetryOptions(TimeSpan.FromSeconds(2), 3) { Handle = ex => ex is InvalidProgramException };

      foreach (string city in request.CityNames)
        tasks.Add(context.CallActivityWithRetryAsync<string>(nameof(Activity_SayHello), retryOptions, city));

      IEnumerable<string> results = Enumerable.Empty<string>();

      try
      {
        results = await Task.WhenAll(tasks);
      }
      catch (Exception ex)
      {
        if (!context.IsReplaying)
          log.LogError(ex, "Error during to call say hello.");
      }

      // --> CallSubOrchestrator
      _ = await context.CallSubOrchestratorAsync<IEnumerable<string>>(nameof(DurableFuncForIsReplaying.Orchestrator_DurableFuncForIsReplaying), null);

      return results;
    }

    [FunctionName(nameof(Activity_SayHello))]
    public static async Task<string> Activity_SayHello(
      [ActivityTrigger] string city,
      ILogger log)
    {
      if (_random.NextDouble() < 0.05 && city.Contains("a"))
        throw new InvalidProgramException($"Random exception for {city}.");

      log.LogInformation($"Wait and saying hello to {city}.");

      await Task.Delay(3000);

      return $"Hello {city}!";
    }
  }
}