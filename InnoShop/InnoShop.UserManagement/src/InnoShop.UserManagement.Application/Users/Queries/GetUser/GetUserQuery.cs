using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;

using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUser;

[Authorize(Permissions = AppPermissions.User.Read)]
public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<User>>;

