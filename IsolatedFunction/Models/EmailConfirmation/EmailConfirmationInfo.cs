namespace IsolatedFunction.Models.EmailConfirmation;

public sealed class EmailConfirmationInfo
{
    public string Id { get; set; }
    public Uri RequestUri { get; set; }
    public string Email { get; set; }
    public string OrchestrationId { get; set; }
    public string Result { get; set; }
}
