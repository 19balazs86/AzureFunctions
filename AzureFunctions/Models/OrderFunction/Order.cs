namespace AzureFunctions.Models.OrderFunction;

public class Order
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}
