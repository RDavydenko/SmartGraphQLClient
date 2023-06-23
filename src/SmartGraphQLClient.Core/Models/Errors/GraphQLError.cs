using System.Text.Json.Serialization;

namespace SmartGraphQLClient.Errors
{
    public class GraphQLError
    {
        [JsonInclude] public string Message { get; private set; } = string.Empty;
        [JsonInclude] public string? Code { get; private set; }
        [JsonInclude] public IReadOnlyDictionary<string, string>? Extensions { get; private set; }
        [JsonInclude] public IReadOnlyList<Location>? Locations { get; private set; }
    }
}
