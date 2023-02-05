using Azure;
using Azure.Data.Tables;

namespace IsolatedFunction.Models.EmailConfirmation;

public class EmailConfirmationTableEntity : ITableEntity
{
    public const string TableNameValue    = "EmailConfirmations";
    public const string PartitionKeyValue = "confirmation";

    public string Email { get; set; }
    public string Result { get; set; }
    public string OrchestrationId { get; set; }

    public string PartitionKey { get; set; } = PartitionKeyValue;
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set ; }
    public ETag ETag { get; set; }

    public EmailConfirmationTableEntity() { }

    public EmailConfirmationTableEntity(EmailConfirmationInfo confirmationInfo)
    {
        RowKey          = confirmationInfo.Id;
        Email           = confirmationInfo.Email;
        Result          = confirmationInfo.Result;
        OrchestrationId = confirmationInfo.OrchestrationId;
    }
}
