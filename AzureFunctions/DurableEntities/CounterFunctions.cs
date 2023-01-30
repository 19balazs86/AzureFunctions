using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net;

namespace AzureFunctions.DurableEntities;
public static class CounterFunctions
{
    [FunctionName(nameof(CounterAdd))]
    public static async Task<HttpResponseMessage> CounterAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counter/{entityKey}/{amount}")] HttpRequestMessage request,
        [DurableClient] IDurableEntityClient client,
        string entityKey,
        int amount)
    {
        await client.SignalEntityAsync<ICounter>(getEntityId(entityKey), c => c.Add(amount));

        // Scheduled signals (aka "reminders")
        //await client.SignalEntityAsync<ICounter>(getEntityId(entityKey), DateTime.UtcNow.AddSeconds(5), c => c.Add(amount));

        return request.CreateResponse(HttpStatusCode.Accepted);
    }

    [FunctionName(nameof(CounterGet))]
    public static async Task<IActionResult> CounterGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counter/{entityKey}")] HttpRequestMessage request,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
    {
        EntityStateResponse<Counter> state = await client.ReadEntityStateAsync<Counter>(getEntityId(entityKey));

        //return request.CreateResponse(state) // Did not work. ResponseCode 406
        return new OkObjectResult(state);
    }

    [FunctionName(nameof(CounterGetAll))]
    public static async Task<IActionResult> CounterGetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counters")] HttpRequest request,
        [DurableClient] IDurableEntityClient client)
    {
        var entityQuery = new EntityQuery { EntityName = nameof(Counter) };

        EntityQueryResult result = await client.ListEntitiesAsync(entityQuery, request.HttpContext.RequestAborted);

        return new OkObjectResult(result.Entities);
    }

    [FunctionName(nameof(CounterDelete))]
    public static async Task<HttpResponseMessage> CounterDelete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "Counter/{entityKey}")] HttpRequestMessage request,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
    {
        await client.SignalEntityAsync<ICounter>(getEntityId(entityKey), c => c.Delete());

        return request.CreateResponse(HttpStatusCode.Accepted);
    }

    // Example of using DurableEntity with DurableOrchestration
    [FunctionName(nameof(CounterClient))]
    public static async Task<HttpResponseMessage> CounterClient(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "CounterClient/{entityKey}/{amount}")] HttpRequestMessage request,
        [DurableClient] IDurableClient client,
        string entityKey,
        int amount)
    {
        var input = new CounterOrchestratorInput { EntityKey = entityKey, Amount = amount };

        string instanceId = await client.StartNewAsync(nameof(CounterOrchestrator), input);

        return client.CreateCheckStatusResponse(request, instanceId);
    }

    // In the Orchestration method we can get the DurableEntity
    [FunctionName(nameof(CounterOrchestrator))]
    public static async Task<int> CounterOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var input = context.GetInput<CounterOrchestratorInput>();

        ICounter counter = context.CreateEntityProxy<ICounter>(input.EntityKey); // EntityId also works as parameter.

        // Two-way call to the entity which awaits the response value.
        return await counter.Add(input.Amount);
    }

    private static EntityId getEntityId(string entityKey) => new EntityId(nameof(Counter), entityKey);
}
