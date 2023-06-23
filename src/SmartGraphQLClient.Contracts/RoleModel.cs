using SmartGraphQLClient.Contracts.Abstractions;

namespace SmartGraphQLClient.Contracts
{
    public class RoleModel : ModelBase
    {
        public RoleCode Code { get; set; } = RoleCode.UNDEFINED;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public UserModel[] Users { get; set; } = Array.Empty<UserModel>();
    }
}
