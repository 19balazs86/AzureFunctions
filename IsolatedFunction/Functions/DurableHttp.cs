using Microsoft.Azure.Functions.Worker.Extensions.DurableTask.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

namespace IsolatedFunction.Functions;

public static class DurableHttp
{
    private static readonly Uri _url = new Uri("https://jsonplaceholder.typicode.com/posts/1");

    [Function(nameof(Client_StartDurableHttp))]
    public static async Task<HttpResponseData> Client_StartDurableHttp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request,
        [DurableClient] DurableTaskClient taskClient)
    {
        string instanceId = await taskClient.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator_CallDurableHttp));

        return taskClient.CreateCheckStatusResponse(request, instanceId);
    }

    [Function(nameof(Orchestrator_CallDurableHttp))]
    public static async Task<string?> Orchestrator_CallDurableHttp([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        DurableHttpResponse response = await context.CallHttpAsync(HttpMethod.Get, _url);

        if (response.StatusCode == HttpStatusCode.OK)
            return response.Content;

        return "Something went wrong";
    }
}