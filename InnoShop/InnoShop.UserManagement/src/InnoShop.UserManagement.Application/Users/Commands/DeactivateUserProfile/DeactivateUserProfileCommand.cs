using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Deactivate)]
public record DeactivateUserProfileCommand(
    Guid UserId
) : IRequest<ErrorOr<Success>>; 