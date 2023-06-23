using HotChocolate.Authorization;
using SmartGraphQLClient.Contracts;
using SmartGraphQLClient.GraphQLServer.Services.Abstractions;

namespace SmartGraphQLClient.GraphQLServer.GraphQL
{
    public class Query
    {
        [AllowAnonymous]
        [UseFirstOrDefault]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<UserModel> GetUser([Service] IUnitOfWork unitOfWork, int? id = null)
        {
            if (id.HasValue && id.Value <= 0) throw new ArgumentException("Id must be greater than zero", nameof(id));

            return id.HasValue
                   ? unitOfWork.Users.Where(x => x.Id == id.Value)
                   : unitOfWork.Users;
        }

        [AllowAnonymous]
        public int GetInteger(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than zero", nameof(id));

            return id;
        }

        [AllowAnonymous]
        [UseFirstOrDefault]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<RoleModel> GetRole([Service] IUnitOfWork unitOfWork, int? id = null)
            => id.HasValue
                ? unitOfWork.Roles.Where(x => x.Id == id.Value)
                : unitOfWork.Roles;

        [AllowAnonymous]
        [UseFirstOrDefault]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PositionModel> GetPosition([Service] IUnitOfWork unitOfWork, int? id = null)
            => id.HasValue
                ? unitOfWork.Positions.Where(x => x.Id == id.Value)
                : unitOfWork.Positions;

        [AllowAnonymous]
        //[UsePaging]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<UserModel> GetUsersPage([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Users;

        [AllowAnonymous]
        //[UsePaging]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<RoleModel> GetRolesPage([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Roles;

        [AllowAnonymous]
        //[UsePaging]
        [UseOffsetPaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PositionModel> GetPositionsPage([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Positions;

        [AllowAnonymous]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<UserModel> GetUsers([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Users;

        [AllowAnonymous]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<RoleModel> GetRoles([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Roles;

        [AllowAnonymous]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PositionModel> GetPositions([Service] IUnitOfWork unitOfWork)
            => unitOfWork.Positions;
    }
}
