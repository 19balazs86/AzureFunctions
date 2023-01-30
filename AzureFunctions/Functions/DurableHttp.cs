using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net;

namespace AzureFunctions.Functions;

public static class DurableHttp
{
    private static readonly Uri _uri = new Uri("https://jsonplaceholder.typicode.com/posts/1");

    [FunctionName(nameof(Client_StartDurableHttp))]
    public static async Task<HttpResponseMessage> Client_StartDurableHttp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage request,
        [DurableClient] IDurableClient starter)
    {
        string instanceId = await starter.StartNewAsync(nameof(Orchestrator_CallDurableHttp));

        return starter.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(nameof(Orchestrator_CallDurableHttp))]
    public static async Task<string> Orchestrator_CallDurableHttp([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        DurableHttpResponse response = await context.CallHttpAsync(HttpMethod.Get, _uri);

        if (response.StatusCode == HttpStatusCode.OK)
            return response.Content;

        return "Something went wrong";
    }
}