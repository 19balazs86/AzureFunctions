namespace AzureFunctions.Models;

public class SayHelloRequest
{
    public IEnumerable<string> CityNames { get; set; }
}
