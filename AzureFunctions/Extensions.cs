using AzureFunctions.Models.OrderFunction;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AzureFunctions;

public static class Extensions
{
    private static readonly JsonSerializer _jsonSerializer = JsonSerializer.Create();

    public static T ReadAs<T>(this Stream stream)
    {
        using var streamReader   = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);

        return _jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    public static async Task WriteJsonAsync(this TextWriter textWriter, object value)
    {
        using var jsonWriter = new JsonTextWriter(textWriter);

        _jsonSerializer.Serialize(jsonWriter, value);

        await jsonWriter.FlushAsync();

        // Do not forget to flush the JsonTextWriter or you’ll end with an empty stream.
        // Efficient post calls with HttpClient and JSON.NET
        // https://johnthiriet.com/efficient-post-calls
    }

    public static Task<HttpRequestBody<T>> GetBodyAsync<T>(this HttpRequest request) where T : class
    {
        T value;

        try
        {
            value = request.Body.ReadAs<T>();
        }
        catch (Exception ex)
        {
            return Task.FromResult(HttpRequestBody<T>.CreateInvalid(ex.Message));
        }

        if (value is null)
            return Task.FromResult(HttpRequestBody<T>.CreateInvalid("Request body can not be empty."));

        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(value, new ValidationContext(value, null, null), validationResults, true);

        return Task.FromResult(new HttpRequestBody<T>(isValid, value, validationResults));
    }
}
