using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Models.OrderFunction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
    public class OrderFunctions
  {
    private static readonly Random _random = new Random();

    private readonly IHttpClientFactory _clientFactory;

    public OrderFunctions(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    /// <summary>
    /// TimerTrigger -> HttpTrigger
    /// </summary>
    [FunctionName("TimerFunction")]
    public async Task TimerFunction( // Function is disabled in the local.settings.json
      [TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
    {
      if (myTimer.IsPastDue)
        log.LogInformation("Timer is running late!");

      var orderRequest = new OrderRequest
      {
        CustomerId  = _random.Next(1, 10),
        Quantity    = _random.Next(1, 10),
        ProductName = $"Product #{_random.Next(1, 10)}"
      };

      _ = await _clientFactory.CreateClient(Startup.PlaceOrderClientName)
        .PostAsJsonAsync("api/PlaceOrder", orderRequest);
    }

    /// <summary>
    /// HttpTrigger -> Write into blob storage.
    /// </summary>
    [FunctionName("HttpFunction")]
    public static async Task<IActionResult> HttpFunction(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PlaceOrder")] HttpRequest request,
      //[Blob("orders/{rand-guid}.json")] TextWriter textWriter,
      Binder binder,
      ILogger log)
    {
      HttpRequestBody<OrderRequest> requestBody = await request.GetBodyAsync<OrderRequest>();

      if (!requestBody.IsValid)
        return new BadRequestObjectResult($"Model is invalid: {requestBody.ValidationString()}");

      var order = new Order
      {
        Id          = Guid.NewGuid(),
        Date        = DateTime.UtcNow,
        CustomerId  = requestBody.Value.CustomerId,
        ProductName = requestBody.Value.ProductName,
        Quantity    = requestBody.Value.Quantity
      };

      log.LogInformation("Order is requested with id: {orderId}.", order.Id);

      string fileName = $"orders/{order.Id}.json";

      // Create binding imperatively.
      using (var textWriter = await binder.BindAsync<TextWriter>(new BlobAttribute(fileName, FileAccess.Write)))
        await textWriter.WriteJsonAsync(order);

      return new OkObjectResult(new { Message = "Order accepted.", order.Id });
    }

    /// <summary>
    /// BlobTrigger -> Queue.
    /// </summary>
    /// <returns></returns>
    [FunctionName("BlobFunction")]
    [return: Queue("orders")]
    public static async Task<Order> BlobFunction(
      [BlobTrigger("orders/{fileName}")] CloudBlockBlob myBlob,
      string fileName,
      //[Queue("orders")] IAsyncCollector<Order> ordersQueue,
      ILogger log)
    {
      log.LogInformation($"Blob trigger function processing the file: '{fileName}'.");

      Order order;

      using (Stream stream = await myBlob.OpenReadAsync())
        order = stream.ReadAs<Order>();

      // If the file is processed but is not deleted, next time it won't be processed again.
      // The blob container has information about the processed messages: azure-webjobs-hosts/blobreceipts
      await myBlob.DeleteAsync();

      log.LogInformation($"Blob trigger function processed the order({order.Id}).");

      return order;
    }

    [FunctionName("QueueFunction")]
    public static void QueueFunction( // Async methods cannot have ref, in or out parameters.
      [QueueTrigger("orders")] Order order,
      //[Table("orders")] CloudTable orderTable,
      [Table("orders")] out OrderTableEntity tableEntity,
      ILogger log)
    {
      log.LogInformation($"Queue trigger function processed the order({order.Id}).");

      tableEntity = new OrderTableEntity(order);

      //TableOperation operation = TableOperation.InsertOrReplace(tableEntity);
      //TableResult result       = await orderTable.ExecuteAsync(operation);
    }
  }
}
