using IsolatedFunction.Functions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IsolatedFunction;

public static class Program
{
    public static void Main(string[] args)
    {
        // Middleware examples
        // https://www.adamstorr.co.uk/blog/conditional-middleware-in-isolated-azure-functions
        // https://github.com/Azure/azure-functions-dotnet-worker/tree/main/samples/CustomMiddleware
        // https://techiesweb.net/2022/05/15/azure-functions-isolated-middleware-apis

        IHost host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(configureServices)
            .Build();

        host.Run();
    }

    private static void configureServices(HostBuilderContext builderContext, IServiceCollection services)
    {
        services.AddHttpClient(OrderFunctions.PlaceOrderClientName, client => client.BaseAddress = new Uri("http://localhost:5000"));

        string storageConnString = builderContext.Configuration["AzureWebJobsStorage"];

        // Azure .NET SDKs
        // https://azure.github.io/azure-sdk/releases/latest/dotnet.html

        // Example: https://kaylumah.nl/2022/02/21/working-with-azure-sdk-for-dotnet.html
        // For DI container using the package: Microsoft.Extensions.Azure

        services.AddAzureClients(clients =>
        {
            clients.AddBlobServiceClient(storageConnString);
            clients.AddTableServiceClient(storageConnString);
        });
    }
}
