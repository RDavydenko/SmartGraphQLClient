using SmartGraphQLClient.Contracts.Abstractions;

namespace SmartGraphQLClient.Contracts
{
    public class PositionModel : ModelBase
    {
        public string Name { get; set; } = string.Empty;
        public UserModel[] Users { get; set; } = Array.Empty<UserModel>();
    }
}
