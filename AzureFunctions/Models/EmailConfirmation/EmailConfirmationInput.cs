using System;

namespace AzureFunctions.Models.EmailConfirmation
{
  public class EmailConfirmationInput
  {
    public string Email { get; set; }
    public Uri RequestUri { get; set; }

    public EmailConfirmationInput() { }

    public EmailConfirmationInput(string email, Uri requestUri)
    {
      Email      = email;
      RequestUri = requestUri;
    }
  }
}
