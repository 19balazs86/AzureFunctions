using System;

namespace AzureFunctions.Models.EmailConfirmation
{
  public class EmailConfirmationInfo
  {
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string OrchestrationId { get; set; }
    public string Result { get; set; }
  }
}
