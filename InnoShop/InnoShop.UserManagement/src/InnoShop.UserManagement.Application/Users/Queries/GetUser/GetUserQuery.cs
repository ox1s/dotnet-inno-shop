using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Users;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUser;

[Authorize(Permissions = AppPermissions.User.Read)]
public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<UserResponse>>;