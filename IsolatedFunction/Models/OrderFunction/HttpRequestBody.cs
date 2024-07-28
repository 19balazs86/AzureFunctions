using System.ComponentModel.DataAnnotations;

namespace IsolatedFunction.Models.OrderFunction;

public sealed class HttpRequestBody<T> where T : class
{
    public bool IsValid { get; private set; }

    public T? Value { get; private set; }

    public IEnumerable<ValidationResult> ValidationResults { get; private set; } = [];

    public HttpRequestBody(bool isValid, T? value, IEnumerable<ValidationResult>? validationResults = null)
    {
        IsValid           = isValid;
        Value             = value;
        ValidationResults = validationResults ?? [];
    }

    public string ValidationString() => string.Join(", ", ValidationResults.Select(v => v.ErrorMessage));

    public static HttpRequestBody<T> CreateInvalid(string errorMessage)
    {
        return new HttpRequestBody<T>(false, null, [new ValidationResult(errorMessage)]);
    }

    public async Task WriteProblemDetails(HttpResponseData response)
    {
        var problemDetails = new
        {
            Title      = "Validation failed",
            Detail     = ValidationString(),
            StatusCode = (int)HttpStatusCode.UnprocessableEntity,

        };

        await response.WriteAsJsonAsync(problemDetails);

        response.StatusCode = HttpStatusCode.UnprocessableEntity;
    }
}
