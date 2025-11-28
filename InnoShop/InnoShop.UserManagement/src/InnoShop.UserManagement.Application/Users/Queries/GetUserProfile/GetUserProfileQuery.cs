using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Read)]
public record GetUserProfileQuery(Guid UserId) : IRequest<ErrorOr<User>>;