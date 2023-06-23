using HotChocolate.Authorization;
using SmartGraphQLClient.Contracts;
using SmartGraphQLClient.GraphQLServer.Services.Abstractions;

namespace SmartGraphQLClient.GraphQLServer.GraphQL
{
    [Authorize]
    [ExtendObjectType("Query")]
    public class AuthorizedQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<UserModel> GetAdmins([Service] IUnitOfWork unitOfWork)
        {
            return unitOfWork.Users.Where(x => x.Roles.Any(m => m.Code == RoleCode.ADMINISTRATOR));
        }
    }
}
