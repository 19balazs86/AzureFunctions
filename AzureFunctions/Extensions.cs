using System.IO;
using Newtonsoft.Json;

namespace AzureFunctions
{
  public static class Extensions
  {
    private static readonly JsonSerializer _jsonSerializer = JsonSerializer.Create();

    public static T Deserialize<T>(this Stream stream)
    {
      using (var streamReader   = new StreamReader(stream))
      using (var jsonTextReader = new JsonTextReader(streamReader))
        return _jsonSerializer.Deserialize<T>(jsonTextReader);
    }
  }
}
