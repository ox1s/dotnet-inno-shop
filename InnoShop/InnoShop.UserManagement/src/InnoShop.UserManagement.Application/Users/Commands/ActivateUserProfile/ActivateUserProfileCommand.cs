using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Security;

using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.ActivateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Activate)]
public record ActivateUserProfileCommand(
    Guid UserId
) : IRequest<ErrorOr<Success>>;