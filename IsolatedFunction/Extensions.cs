using Azure.Data.Tables;
using IsolatedFunction.Models.OrderFunction;
using System.ComponentModel.DataAnnotations;

namespace IsolatedFunction;

public static class Extensions
{
    public static async Task<TableClient> GetTableClient_CreateIfNotExistsAsync(this TableServiceClient tableServiceClient, string tableName)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName, nameof(tableName));

        TableClient tableClient = tableServiceClient.GetTableClient(tableName);

        await tableClient.CreateIfNotExistsAsync();

        return tableClient;
    }

    public static async Task<HttpResponseData> CreateStringResponseAsync(
        this HttpRequestData request,
        string value,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        HttpResponseData response = request.CreateResponse(statusCode);

        await response.WriteStringAsync(value);

        return response;
    }

    public static async Task<HttpResponseData> CreateJsonResponseAsync<TData>(this HttpRequestData request, TData data)
    {
        HttpResponseData response = request.CreateResponse();

        await response.WriteAsJsonAsync(data);

        return response;
    }

    public static async Task<HttpRequestBody<T>> GetBody<T>(this HttpRequestData request) where T : class
    {
        T? value;

        try
        {
            value = await request.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            return HttpRequestBody<T>.CreateInvalid(ex.Message);
        }

        if (value is null)
        {
            return HttpRequestBody<T>.CreateInvalid("Request body can not be empty.");
        }

        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(value, new ValidationContext(value, null, null), validationResults, true);

        return new HttpRequestBody<T>(isValid, value, validationResults);
    }
}
