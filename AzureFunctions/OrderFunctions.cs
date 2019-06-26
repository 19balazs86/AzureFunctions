using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
  public static class OrderFunctions
  {
    private static readonly JsonSerializer _jsonSerializer = JsonSerializer.Create();

    /// <summary>
    /// HttpTrigger -> Write into blob storage.
    /// </summary>
    [FunctionName("Order")]
    public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
      //[Blob("orders/{rand-guid}.json")] TextWriter textWriter,
      Binder binder,
      ILogger log)
    {
      OrderRequest orderRequest;

      try
      {
        orderRequest = request.Body.Deserialize<OrderRequest>();
      }
      catch (Exception ex)
      {
        log.LogError(ex, "Failed to deserialize the order.");

        return new BadRequestObjectResult("The order is invalid.");
      }

      var order = new Order
      {
        Id          = Guid.NewGuid(),
        Date        = DateTime.UtcNow,
        UserId      = orderRequest.UserId,
        ProductName = orderRequest.ProductName,
        Quantity    = orderRequest.Quantity
      };

      order.Id   = Guid.NewGuid();
      order.Date = DateTime.UtcNow;

      log.LogInformation("Order is requested with id: {orderId}.", order.Id);

      string fileName = $"orders/{order.Id}.json";

      // Create binding imperatively.
      using (var textWriter = await binder.BindAsync<TextWriter>(new BlobAttribute(fileName, FileAccess.Write)))
      using (var jsonWriter = new JsonTextWriter(textWriter))
        _jsonSerializer.Serialize(jsonWriter, order);

      return new OkObjectResult(new { Message = "Order accepted.", order.Id });
    }
  }
}
