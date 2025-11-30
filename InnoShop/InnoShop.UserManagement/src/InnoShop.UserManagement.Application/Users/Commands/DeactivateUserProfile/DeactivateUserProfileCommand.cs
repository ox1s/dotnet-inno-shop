using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Deactivate)]
public record DeactivateUserProfileCommand(
    Guid UserId
) : IAuthorizeableRequest<ErrorOr<Success>>;