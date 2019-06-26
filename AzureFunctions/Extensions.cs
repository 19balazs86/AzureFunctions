using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AzureFunctions
{
  public static class Extensions
  {
    private static readonly JsonSerializer _jsonSerializer = JsonSerializer.Create();

    public static T ReadAs<T>(this Stream stream)
    {
      using (var streamReader   = new StreamReader(stream))
      using (var jsonTextReader = new JsonTextReader(streamReader))
        return _jsonSerializer.Deserialize<T>(jsonTextReader);
    }

    public static void WriteJson(this TextWriter textWriter, object value)
    {
      using (var jsonWriter = new JsonTextWriter(textWriter))
        _jsonSerializer.Serialize(jsonWriter, value);
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
        return Task.FromResult(new HttpRequestBody<T>(ex.Message));
      }

      if (value is null)
        return Task.FromResult(new HttpRequestBody<T>("Request body can not be empty."));

      var validationResults = new List<ValidationResult>();

      bool isValid = Validator.TryValidateObject(value, new ValidationContext(value, null, null), validationResults, true);

      return Task.FromResult(new HttpRequestBody<T>(isValid, value, validationResults));
    }
  }
}
