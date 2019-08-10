using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
  public static class SayHelloDurableFunctions
  {
    [FunctionName(nameof(SayHello_Client))]
    public static async Task<HttpResponseMessage> SayHello_Client(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage request,
      [OrchestrationClient] DurableOrchestrationClient starter,
      ILogger log)
    {
      var sayHelloRequest = await request.Content.ReadAsAsync<SayHelloRequest>();

      string instanceId = await starter.StartNewAsync(nameof(SayHello_Orchestrator), sayHelloRequest);

      log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

      return starter.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(nameof(SayHello_Orchestrator))]
    public static async Task<IEnumerable<string>> SayHello_Orchestrator(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log)
    {
      log.LogInformation($"Run: {nameof(SayHello_Orchestrator)}.");

      // context.CurrentUtcDateTime
      // context.NewGuid()

      var request = context.GetInput<SayHelloRequest>();
      var tasks   = new List<Task<string>>();

      foreach (string city in request.CityNames)
        tasks.Add(context.CallActivityAsync<string>(nameof(SayHello_Activity), city));

      await Task.WhenAll(tasks);

      return tasks.Select(t => t.Result);
    }

    [FunctionName(nameof(SayHello_Activity))]
    public static async Task<string> SayHello_Activity(
      [ActivityTrigger] string city,
      ILogger log)
    {
      log.LogInformation($"Wait and saying hello to {city}.");

      await Task.Delay(5000);

      return $"Hello {city}!";
    }
  }
}