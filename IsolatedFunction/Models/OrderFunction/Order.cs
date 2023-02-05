using Azure.Data.Tables;

namespace IsolatedFunction.Models.OrderFunction;

public class Order
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }

    public TableEntity ToTableEntity()
    {
        return new TableEntity("order", Id.ToString())
        {
            [nameof(CustomerId)]  = CustomerId,
            [nameof(Date)]        = Date,
            [nameof(ProductName)] = ProductName,
            [nameof(Quantity)]    = Quantity
        };
    }
}
