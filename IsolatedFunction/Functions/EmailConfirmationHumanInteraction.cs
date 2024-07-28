using Azure;
using Azure.Data.Tables;
using IsolatedFunction.Models.EmailConfirmation;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using System.Web;

namespace IsolatedFunction.Functions;

public sealed class EmailConfirmationHumanInteraction
{
    private readonly ILogger _logger;

    private readonly TableServiceClient _tableServiceClient;

    private const string _eventName = "EmailConfirmationResult";

    public EmailConfirmationHumanInteraction(
        ILogger<EmailConfirmationHumanInteraction> logger,
        TableServiceClient tableServiceClient)
    {
        _logger             = logger;
        _tableServiceClient = tableServiceClient;
    }

    [Function(nameof(Client_StartEmailConfirmation))]
    public static async Task<HttpResponseData> Client_StartEmailConfirmation(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request,
        [DurableClient] DurableTaskClient taskClient)
    {
        // This process is intended to showcase the features of Durable Functions.

        string? email = HttpUtility.ParseQueryString(request.Url.Query)["email"];

        if (string.IsNullOrWhiteSpace(email))
        {
            return await request.CreateStringResponseAsync("Missing email field in the query.", HttpStatusCode.BadRequest);
        }

        var input = new EmailConfirmationInput(email, request.Url);

        string instanceId = await taskClient.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator_SendEmailConfirmation), input);

        return taskClient.CreateCheckStatusResponse(request, instanceId);
    }

    [Function(nameof(Orchestrator_SendEmailConfirmation))]
    public async Task<EmailConfirmationInfo> Orchestrator_SendEmailConfirmation(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        EmailConfirmationInput input)
    {
        if (!context.IsReplaying)
            _logger.LogInformation("SendEmailConfirmation orchestration started.");

        var confirmationInfo = new EmailConfirmationInfo
        {
            Id              = context.NewGuid().ToString(),
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
                context.SetCustomStatus($"Waiting for human interaction or timeout at: {timeoutAt:HH:mm:ss} UTC.");

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

    [Function(nameof(Activity_SendEmailConfirmation))]
    public async Task Activity_SendEmailConfirmation(
        [ActivityTrigger] EmailConfirmationInfo confirmationInfo)
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = confirmationInfo.RequestUri.Scheme,
            Host   = confirmationInfo.RequestUri.Host,
            Port   = confirmationInfo.RequestUri.Port,
            Path   = $"api/{nameof(ConfirmEmail)}/{confirmationInfo.Id}",
            Query  = "result=Approved"
        };

        TableClient tableClient = await _tableServiceClient.GetTableClient_CreateIfNotExistsAsync(EmailConfirmationTableEntity.TableNameValue);

        _ = await tableClient.AddEntityAsync(new EmailConfirmationTableEntity(confirmationInfo));

        _logger.LogWarning("Confirm email with the '{URL}'.", uriBuilder.ToString());
    }

    [Function(nameof(ConfirmEmail))]
    public async Task<HttpResponseData> ConfirmEmail(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ConfirmEmail/{id}")] HttpRequestData request,
        [DurableClient] DurableTaskClient taskClient,
        string id)
    {
        HttpResponseData response = request.CreateResponse(HttpStatusCode.BadRequest);

        EmailConfirmationTableEntity? tableEntity = await getEmailConfirmationTableEntity(id);

        if (tableEntity is null)
        {
            response.WriteString("Invalid request.");
            return response;
        }
        else if (tableEntity.Result != "Sent")
        {
            response.WriteString("The email is already confirmed.");
            return response;
        }

        string? result = HttpUtility.ParseQueryString(request.Url.Query)["result"]; // This is just an extra parameter.

        // --> Send confirmation external event to this orchestration.
        await taskClient.RaiseEventAsync(tableEntity.OrchestrationId, _eventName, result);

        _logger.LogInformation("RaiseEvent called to confirm email.");

        await response.WriteAsJsonAsync(new { Status = "Email confirmed" });

        return response;
    }

    [Function(nameof(Activity_SaveEmailConfirmationResult))]
    public async Task Activity_SaveEmailConfirmationResult(
        [ActivityTrigger] EmailConfirmationInfo confirmationInfo)
    {
        EmailConfirmationTableEntity? tableEntity = await getEmailConfirmationTableEntity(confirmationInfo.Id);

        if (tableEntity is null)
        {
            throw new NullReferenceException($"Missing email confirmation record.");
        }

        tableEntity.Result = confirmationInfo.Result;

        TableClient tableClient = _tableServiceClient.GetTableClient(EmailConfirmationTableEntity.TableNameValue);

        await tableClient.UpdateEntityAsync(tableEntity, tableEntity.ETag);

        _logger.LogInformation("Result is saved with '{Value}'.", confirmationInfo.Result);
    }

    private async Task<EmailConfirmationTableEntity?> getEmailConfirmationTableEntity(string id)
    {
        TableClient tableClient = await _tableServiceClient.GetTableClient_CreateIfNotExistsAsync(EmailConfirmationTableEntity.TableNameValue);

        NullableResponse<EmailConfirmationTableEntity> nullableTableEntity = await tableClient.GetEntityIfExistsAsync<EmailConfirmationTableEntity>(
            EmailConfirmationTableEntity.PartitionKeyValue, id);

        return nullableTableEntity.HasValue ? nullableTableEntity.Value : null;
    }
}
