namespace AzureFunctions.Models
{
  public class OrderRequest
  {
    public int UserId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
  }
}
