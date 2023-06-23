using SmartGraphQLClient.Contracts.Abstractions;

namespace SmartGraphQLClient.Contracts
{
    public class UserModel : ModelBase
    {
        public string UserName { get; set; } = string.Empty;
        public int Age { get; set; }
        public PositionModel? Position { get; set; }
        public RoleModel[] Roles { get; set; } = Array.Empty<RoleModel>();
    }
}