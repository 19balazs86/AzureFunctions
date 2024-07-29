using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using IsolatedFunction.Functions;
using IsolatedFunction.Models.EmailConfirmation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;

namespace IsolatedFunction.Infrastructure;

public sealed class InfrastructureService : BackgroundService
{
    // Somehow the Azure Function needs this queue. I do not know why it has this name
    // When I run the application, it shows the error: 'The specified queue does not exist.'
    // I used the Visual Studio's Output window, selected Service Dependencies, and found the 404 Not Found log message with this queue name
    private static readonly string _webjobsQueue = $"azure-webjobs-blobtrigger-{Environment.MachineName.ToLower()}-916312667";

    private readonly ImmutableArray<string> _queueNames = [_webjobsQueue, OrderFunctions.QueueName];

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InfrastructureService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        IServiceProvider serviceProvider = scope.ServiceProvider;

        Task[] ensureTasks =
        [
            ensureBlobContainerExists(serviceProvider),
            ensureQueueExists(serviceProvider),
            ensureTableExists(serviceProvider)
        ];

        await Task.WhenAll(ensureTasks);
    }

    private async Task ensureBlobContainerExists(IServiceProvider serviceProvider)
    {
        BlobServiceClient blobServiceClient = serviceProvider.GetRequiredService<BlobServiceClient>();

        BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(OrderFunctions.BlobName);

        await blobContainerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);
    }

    private async Task ensureQueueExists(IServiceProvider serviceProvider)
    {
        QueueServiceClient queueServiceClient = serviceProvider.GetRequiredService<QueueServiceClient>();

        foreach (string queueName in _queueNames)
        {
            QueueClient queueClient = queueServiceClient.GetQueueClient(queueName);

            await queueClient.CreateIfNotExistsAsync();
        }
    }

    private async Task ensureTableExists(IServiceProvider serviceProvider)
    {
        TableServiceClient tableServiceClient = serviceProvider.GetRequiredService<TableServiceClient>();

        await tableServiceClient.CreateTableIfNotExistsAsync(OrderFunctions.TableName);
        await tableServiceClient.CreateTableIfNotExistsAsync(EmailConfirmationTableEntity.TableNameValue);
    }
}
