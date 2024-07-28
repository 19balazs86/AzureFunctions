namespace IsolatedFunction.Models.EmailConfirmation;

public sealed class EmailConfirmationInput
{
    public string Email { get; set; }
    public Uri RequestUri { get; set; }

    public EmailConfirmationInput(string email, Uri requestUri)
    {
        Email      = email;
        RequestUri = requestUri;
    }
}
