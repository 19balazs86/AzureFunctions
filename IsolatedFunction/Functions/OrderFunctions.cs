using Azure.Data.Tables;
using Azure.Storage.Blobs;
using IsolatedFunction.Models.OrderFunction;
using System.Net.Http.Json;

namespace IsolatedFunction.Functions;

public sealed class OrderFunctions
{
    public const string PlaceOrderClientName = "PlaceOrderApi";

    private readonly ILogger _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly TableServiceClient _tableServiceClient;

    public OrderFunctions(
        ILogger<OrderFunctions> logger,
        IHttpClientFactory clientFactory,
        BlobServiceClient blobServiceClient,
        TableServiceClient tableServiceClient)
    {
        _logger             = logger;
        _clientFactory      = clientFactory;
        _blobServiceClient  = blobServiceClient;
        _tableServiceClient = tableServiceClient;
    }

    /// <summary>
    /// TimerTrigger -> HttpTrigger
    /// </summary>
    [Function(nameof(TimerFunction))]
    public async Task TimerFunction( // Function is disabled in the local.settings.json
        [TimerTrigger("*/5 * * * * *")] TimerTriggerInfo myTimer)
    {
        if (myTimer.IsPastDue)
            _logger.LogInformation("Timer is running late!");

        var orderRequest = OrderRequest.CreateRandom();

        _ = await _clientFactory
            .CreateClient(PlaceOrderClientName)
            .PostAsJsonAsync("api/PlaceOrder", orderRequest);
    }

    /// <summary>
    /// HttpTrigger -> Write into blob storage.
    /// </summary>
    [Function(nameof(HttpFunction))]
    public async Task<HttpResponseData> HttpFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PlaceOrder")] HttpRequestData request)
    {
        HttpRequestBody<OrderRequest> requestBody = await request.GetBody<OrderRequest>();

        HttpResponseData response = request.CreateResponse();

        if (!requestBody.IsValid)
        {
            await requestBody.WriteProblemDetails(response);

            return response;
        }

        Order order = requestBody.Value!.ToOrder();

        _logger.LogInformation("Order is requested with id: {orderId}.", order.Id);

        string fileName = $"{order.Id}.json";

        // The Binder feature, which allows for dynamic set up of the blob name, is not available for Isolcated function.
        // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/storage.blobs-readme
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("orders");

        await blobContainerClient.CreateIfNotExistsAsync();

        var binaryData = BinaryData.FromObjectAsJson(order);

        await blobContainerClient.UploadBlobAsync(fileName, binaryData);

        //BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
        //await blobClient.UploadAsync(binaryData);

        await response.WriteAsJsonAsync(new { Message = "Order accepted.", order.Id });

        return response;
    }

    /// <summary>
    /// BlobTrigger -> Queue.
    /// </summary>
    [Function(nameof(BlobFunction))]
    [QueueOutput("orders")]
    public async Task<Order> BlobFunction(
        [BlobTrigger("orders/{fileName}")] Order order,
        string fileName)
    {
        // No need to delete the file. If the it has been processed but hasn't been deleted, it won't be processed again in the future.
        // The processed messages information can be found in the azure-webjobs-hosts/blobreceipts directory of the blob container.
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("orders");

        await blobContainerClient.DeleteBlobAsync(fileName);

        _logger.LogInformation("Blob trigger function processed the order({OrderId}).", order.Id);

        return order;
    }

    /// <summary>
    /// QueueTrigger -> Table.
    /// </summary>
    [Function(nameof(QueueFunction))]
    public async Task QueueFunction(
        [QueueTrigger("orders")] Order order)
    {
        _logger.LogInformation("Queue trigger function processed the order({OrderId}).", order.Id);

        // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme
        TableClient tableClient = await _tableServiceClient.GetTableClient_CreateIfNotExistsAsync("orders");

        _ = await tableClient.AddEntityAsync(order.ToTableEntity());
    }
}
