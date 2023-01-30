using System.ComponentModel.DataAnnotations;

namespace AzureFunctions.Models.OrderFunction;

public class HttpRequestBody<T> where T : class
{
    public bool IsValid { get; private set; }
    public T Value { get; private set; }

    public IEnumerable<ValidationResult> ValidationResults { get; private set; }

    public HttpRequestBody(bool isValid, T value, IEnumerable<ValidationResult> validationResults = null)
    {
        IsValid           = isValid;
        Value             = value;
        ValidationResults = validationResults ?? Enumerable.Empty<ValidationResult>();
    }

    public string ValidationString() => string.Join(", ", ValidationResults.Select(v => v.ErrorMessage));

    public static HttpRequestBody<T> CreateInvalid(string errorMessage)
        => new HttpRequestBody<T>(false, null, new[] { new ValidationResult(errorMessage) });
}
