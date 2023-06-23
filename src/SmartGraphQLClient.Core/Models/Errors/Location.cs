using System.Text.Json.Serialization;

namespace SmartGraphQLClient.Errors
{
    public class Location
    {
        [JsonInclude] public int Line { get; private set; }
        [JsonInclude] public int Column { get; private set; }
    }
}
