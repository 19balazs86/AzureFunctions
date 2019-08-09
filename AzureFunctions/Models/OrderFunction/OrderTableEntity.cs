using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctions.Models.OrderFunction
{
  public class OrderTableEntity : TableEntity
  {
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }

    // public DateTime StoredDate { get; set; } // TableEntity has a Timestamp

    public OrderTableEntity(Order order)
    {
      PartitionKey = "order";
      RowKey       = order.Id.ToString();

      CustomerId  = order.CustomerId;
      Date        = order.Date;
      ProductName = order.ProductName;
      Quantity    = order.Quantity;
    }
  }
}
