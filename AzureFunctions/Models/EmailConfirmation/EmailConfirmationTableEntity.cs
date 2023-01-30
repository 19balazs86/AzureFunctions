using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctions.Models.EmailConfirmation;

public class EmailConfirmationTableEntity : TableEntity
{
    public const string PartitionKeyValue = "confirmation";

    public string Email { get; set; }
    public string Result { get; set; }
    public string OrchestrationId { get; set; }

    public EmailConfirmationTableEntity() { }

    public EmailConfirmationTableEntity(EmailConfirmationInfo confirmationInfo)
        : base(PartitionKeyValue, confirmationInfo.Id.ToString())
    {
        Email           = confirmationInfo.Email;
        Result          = confirmationInfo.Result;
        OrchestrationId = confirmationInfo.OrchestrationId;
    }
}
