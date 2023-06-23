using SmartGraphQLClient.Contracts;

namespace SmartGraphQLClient.GraphQLServer.Services.Abstractions
{
    public interface IUnitOfWork
    {
        IQueryable<UserModel> Users { get; }
        IQueryable<RoleModel> Roles { get; }
        IQueryable<PositionModel> Positions { get; }
    }
}
