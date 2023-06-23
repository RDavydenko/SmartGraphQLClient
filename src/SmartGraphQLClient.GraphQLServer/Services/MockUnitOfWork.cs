using SmartGraphQLClient.Contracts;
using SmartGraphQLClient.GraphQLServer.Database;
using SmartGraphQLClient.GraphQLServer.Services.Abstractions;

namespace SmartGraphQLClient.GraphQLServer.Services
{
    internal class MockUnitOfWork : IUnitOfWork
    {
        public IQueryable<UserModel> Users => DatabaseMock.Users.AsQueryable();
        public IQueryable<RoleModel> Roles => DatabaseMock.Roles.AsQueryable();
        public IQueryable<PositionModel> Positions => DatabaseMock.Positions.AsQueryable();

        //public MockUnitOfWork()
        //{
        //    Users.Include(x => x.Roles)
        //        .ThenInclude(x => x.Users)
        //            .ThenInclude(x => x.)
        //                .ThenInclude(x => x.Code)
        //}
    }
}
