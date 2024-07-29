using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.Entities;
using Microsoft.DurableTask.Entities;

namespace IsolatedFunction.DurableEntities;

public static class CounterFunctions
{
    [Function(nameof(CounterAdd))]
    public static async Task<HttpResponseData> CounterAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counter/{entityKey}/{amount}")] HttpRequestData request,
        [DurableClient] DurableTaskClient client,
        string entityKey,
        int amount)
    {
        await client.Entities.SignalEntityAsync(Counter.CreateEntityId(entityKey), nameof(Counter.Add), amount);

        // Scheduled signals (aka "reminders")
        // var options = new SignalEntityOptions { SignalTime = DateTimeOffset.UtcNow.AddSeconds(5) };
        // await client.Entities.SignalEntityAsync(Counter.CreateEntityId(entityKey), nameof(Counter.Add), amount, options);

        return request.CreateResponse(HttpStatusCode.Accepted);
    }

    [Function(nameof(CounterGet))]
    public static async Task<HttpResponseData> CounterGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counter/{entityKey}")] HttpRequestData request,
        [DurableClient] DurableTaskClient client,
        string entityKey)
    {
        EntityMetadata<CounterState>? entityMetadata = await client.Entities.GetEntityAsync<CounterState>(Counter.CreateEntityId(entityKey));

        if (entityMetadata is null)
        {
            return request.CreateResponse(HttpStatusCode.NotFound);
        }

        return await request.CreateJsonResponseAsync(entityMetadata);
    }

    [Function(nameof(CounterGetAll))]
    public static async Task<HttpResponseData> CounterGetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "Counters")] HttpRequestData request,
        [DurableClient] DurableTaskClient client)
    {
        var entityQuery = new EntityQuery { InstanceIdStartsWith = nameof(Counter), PageSize = 20 };

        AsyncPageable<EntityMetadata<CounterState>> result = client.Entities.GetAllEntitiesAsync<CounterState>(entityQuery);

        List<EntityMetadata<CounterState>> responseList = [];

        await foreach (EntityMetadata<CounterState> entity in result)
        {
            responseList.Add(entity);
        }

        return await request.CreateJsonResponseAsync(responseList);
    }

    [Function(nameof(CounterDelete))]
    public static async Task<HttpResponseData> CounterDelete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "Counter/{entityKey}")] HttpRequestData request,
        [DurableClient] DurableTaskClient client,
        string entityKey)
    {
        // When deriving from TaskEntity<TState>, delete is implicitly defined
        // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities?pivots=isolated#deleting-entities-in-the-isolated-model
        await client.Entities.SignalEntityAsync(Counter.CreateEntityId(entityKey), "Delete"); // Counter
        // Note: This won't hard delete the record. It clears CustomState and Input columns

        return request.CreateResponse(HttpStatusCode.Accepted);
    }

    // Example of using DurableEntity with Orchestration
    [Function(nameof(CounterClient))]
    public static async Task<HttpResponseData> CounterClient(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "CounterClient/{entityKey}/{amount}")] HttpRequestData request,
        [DurableClient] DurableTaskClient client,
        string entityKey,
        int amount)
    {
        var input = new CounterOrchestratorInput { EntityKey = Counter.CreateEntityId(entityKey).ToString(), Amount = amount };

        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(CounterOrchestrator), input);

        return client.CreateCheckStatusResponse(request, instanceId);
    }

    // In the Orchestration method we can get the DurableEntity
    [Function(nameof(CounterOrchestrator))]
    public static async Task<int> CounterOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        CounterOrchestratorInput input = context.GetInput<CounterOrchestratorInput>()
            ?? throw new NullReferenceException("CounterOrchestratorInput is missing!");

        EntityInstanceId counterId = EntityInstanceId.FromString(input.EntityKey!);

        // Two-way call to the entity which awaits the response value
        int counterValue = await context.Entities.CallEntityAsync<int>(counterId, nameof(Counter.Add), input.Amount);

        return counterValue;
    }
}
