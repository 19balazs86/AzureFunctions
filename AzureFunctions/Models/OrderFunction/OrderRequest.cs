using System.ComponentModel.DataAnnotations;

namespace AzureFunctions.Models.OrderFunction;

public class OrderRequest
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string ProductName { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public static OrderRequest CreateRandom()
    {
        return new OrderRequest
        {
            CustomerId  = Random.Shared.Next(1, 10),
            Quantity    = Random.Shared.Next(1, 10),
            ProductName = $"Product #{Random.Shared.Next(1, 10)}"
        };
    }

    public Order ToOrder()
    {
        return new Order
        {
            Id          = Guid.NewGuid(),
            Date        = DateTime.UtcNow,
            CustomerId  = CustomerId,
            ProductName = ProductName,
            Quantity    = Quantity
        };
    }
}
