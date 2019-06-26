using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFunctions
{
  public static class OrderFunctions
  {
    /// <summary>
    /// HttpTrigger -> Write into blob storage.
    /// </summary>
    [FunctionName("Order")]
    public static async Task<IActionResult> HttpFunction(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
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
        UserId      = requestBody.Value.UserId,
        ProductName = requestBody.Value.ProductName,
        Quantity    = requestBody.Value.Quantity
      };

      log.LogInformation("Order is requested with id: {orderId}.", order.Id);

      string fileName = $"orders/{order.Id}.json";

      // Create binding imperatively.
      using (var textWriter = await binder.BindAsync<TextWriter>(new BlobAttribute(fileName, FileAccess.Write)))
        textWriter.WriteJson(order);

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
      Order order;

      using (Stream stream = await myBlob.OpenReadAsync())
      {
        log.LogInformation($"Blob trigger function processing the file: '{fileName}'.");

        order = stream.ReadAs<Order>();
      }
      
      // If the file is processed but is not deleted, next time it won't be processed again.
      // The blob container has information about the processed messages: azure-webjobs-hosts/blobreceipts
      await myBlob.DeleteAsync();

      log.LogInformation($"Blob trigger function processed the order({order.Id}).");

      return order;
    }
  }
}
