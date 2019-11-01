using System;
using AzureFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace AzureFunctions
{
  public class Startup : IWebJobsStartup
  {
    public const string PlaceOrderClientName = "PlaceOrderApi";

    public void Configure(IWebJobsBuilder builder)
    {
      configureServices(builder.Services);
    }

    private static void configureServices(IServiceCollection services)
    {
      services.AddHttpClient(PlaceOrderClientName, client =>
        client.BaseAddress = new Uri("http://localhost:5000"));

    }
  }
}
