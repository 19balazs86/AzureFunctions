using System.ComponentModel.DataAnnotations;

namespace AzureFunctions.Models.OrderFunction
{
  public class OrderRequest
  {
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string ProductName { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
  }
}
