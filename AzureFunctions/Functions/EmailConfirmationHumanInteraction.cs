using AzureFunctions.Models.EmailConfirmation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctions.Functions;

public static class EmailConfirmationHumanInteraction
{
    private const string _eventName = "EmailConfirmationResult";

    [FunctionName(nameof(Client_StartEmailConfirmation))]
    public static async Task<HttpResponseMessage> Client_StartEmailConfirmation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage request,
        [DurableClient] IDurableClient starter)
    {
        // All this process is just for demo purpose to take advantage of the Durable Functions features.

        string email = request.RequestUri.ParseQueryString()["email"];

        if (string.IsNullOrWhiteSpace(email))
            return request.CreateResponse(HttpStatusCode.BadRequest, "Missing email field in the query.");

        var input = new EmailConfirmationInput(email, request.RequestUri);

        string instanceId = await starter.StartNewAsync(nameof(Orchestrator_SendEmailConfirmation), input);

        return starter.CreateCheckStatusResponse(request, instanceId);
    }

    [FunctionName(nameof(Orchestrator_SendEmailConfirmation))]
    public static async Task<EmailConfirmationInfo> Orchestrator_SendEmailConfirmation(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        ILogger log)
    {
        if (!context.IsReplaying)
            log.LogInformation("SendEmailConfirmation orchestration started.");

        EmailConfirmationInput input = context.GetInput<EmailConfirmationInput>();

        var confirmationInfo = new EmailConfirmationInfo
        {
            Id              = context.NewGuid(),
            RequestUri      = input.RequestUri,
            Email           = input.Email,
            OrchestrationId = context.InstanceId,
            Result          = "Sent"
        };

        await context.CallActivityAsync(nameof(Activity_SendEmailConfirmation), confirmationInfo);

        using (var cts = new CancellationTokenSource())
        {
            DateTime timeoutAt = context.CurrentUtcDateTime.AddSeconds(30);
            Task timeoutTask   = context.CreateTimer(timeoutAt, cts.Token);

            Task<string> confirmResultTask = context.WaitForExternalEvent<string>(_eventName);

            if (!context.IsReplaying)
                context.SetCustomStatus($"Waiting for human interaction or timeout at: {timeoutAt.ToString("hh:mm:ss")} UTC.");

            Task winnerTask = await Task.WhenAny(confirmResultTask, timeoutTask);

            if (winnerTask == confirmResultTask)
            {
                cts.Cancel();
                confirmationInfo.Result = confirmResultTask.Result;
            }
            else confirmationInfo.Result = "Timed Out";
        }

        await context.CallActivityAsync(nameof(Activity_SaveEmailConfirmationResult), confirmationInfo);

        return confirmationInfo;
    }

    [FunctionName(nameof(Activity_SendEmailConfirmation))]
    public static void Activity_SendEmailConfirmation(
        [ActivityTrigger] EmailConfirmationInfo confirmationInfo,
        [Table("EmailConfirmations")] out EmailConfirmationTableEntity tableEntity,
        ILogger log)
    {
        tableEntity = new EmailConfirmationTableEntity(confirmationInfo);

        UriBuilder uriBuilder = new UriBuilder
        {
            Scheme = confirmationInfo.RequestUri.Scheme,
            Host   = confirmationInfo.RequestUri.Host,
            Port   = confirmationInfo.RequestUri.Port,
            Path   = $"api/{nameof(ConfirmEmail)}/{confirmationInfo.Id}",
            Query  = "result=Approved"
        };

        log.LogInformation($"Confirm email with the URL: '{uriBuilder}'.");
    }

    [FunctionName(nameof(ConfirmEmail))]
    public static async Task<IActionResult> ConfirmEmail(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ConfirmEmail/{id}")] HttpRequest request,
        [DurableClient] IDurableClient client,
        [Table("EmailConfirmations", EmailConfirmationTableEntity.PartitionKeyValue, "{id}")] EmailConfirmationTableEntity tableEntity,
        ILogger log)
    {
        if (tableEntity is null)
            return new BadRequestObjectResult("Invalid request.");
        else if (tableEntity.Result != "Sent")
            return new OkObjectResult("The email is already confirmed.");

        string result = request.Query["result"]; // This is just an extra parameter.

        //--> Send confirmation external event to this orchestration.
        await client.RaiseEventAsync(tableEntity.OrchestrationId, _eventName, result);

        log.LogInformation("RaiseEvent called to confirm email.");

        return new OkObjectResult("Email confirmed.");
    }

    [FunctionName(nameof(Activity_SaveEmailConfirmationResult))]
    public static async Task Activity_SaveEmailConfirmationResult(
        [ActivityTrigger] EmailConfirmationInfo confirmationInfo,
        [Table("EmailConfirmations")] CloudTable orderTable,
        ILogger log)
    {
        TableOperation operation = TableOperation.Retrieve<EmailConfirmationTableEntity>(
            EmailConfirmationTableEntity.PartitionKeyValue, confirmationInfo.Id.ToString());

        TableResult result = await orderTable.ExecuteAsync(operation);

        var tableEntity = result.Result as EmailConfirmationTableEntity;

        if (tableEntity is null)
            throw new NullReferenceException($"Missing email confirmation record.");

        tableEntity.Result = confirmationInfo.Result;

        operation = TableOperation.Replace(tableEntity);

        _ = await orderTable.ExecuteAsync(operation);

        log.LogInformation($"Result is saved with value: '{confirmationInfo.Result}'.");
    }
}
