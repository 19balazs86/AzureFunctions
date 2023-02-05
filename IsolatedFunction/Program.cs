using IsolatedFunction.Functions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Isolated functions middleware example
// https://techiesweb.net/2022/05/15/azure-functions-isolated-middleware-apis.html
// https://github.com/Azure/azure-functions-dotnet-worker/tree/main/samples/CustomMiddleware

IHost host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(configureServices)
    .Build();

host.Run();

static void configureServices(HostBuilderContext builderContext, IServiceCollection services)
{
    services.AddHttpClient(OrderFunctions.PlaceOrderClientName, client =>
        client.BaseAddress = new Uri("http://localhost:5000"));

    string storageConnString = builderContext.Configuration["AzureWebJobsStorage"];

    services.AddAzureClients(clients =>
    {
        clients.AddBlobServiceClient(storageConnString);
        clients.AddTableServiceClient(storageConnString);
    });
}
