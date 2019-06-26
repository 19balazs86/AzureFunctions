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
      using (var jsonWriter = new JsonTextWriter(textWriter))
        _jsonSerializer.Serialize(jsonWriter, order);

      return new OkObjectResult(new { Message = "Order accepted.", order.Id });
    }
  }
}
