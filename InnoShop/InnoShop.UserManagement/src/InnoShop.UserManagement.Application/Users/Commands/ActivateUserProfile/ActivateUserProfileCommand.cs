using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Application.Users.Commands.ActivateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Activate)]
public record ActivateUserProfileCommand(
    Guid UserId
) : IAuthorizeableRequest<ErrorOr<Success>>;