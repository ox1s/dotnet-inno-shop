using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Users;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Create, Policies = AppPolicies.SelfOrAdmin)]
public record CreateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string AvatarUrl,
    string PhoneNumber,
    Country Country,
    string State,
    string City)
    : IAuthorizeableRequest<ErrorOr<UserProfileResponse>>, IInvalidatesUserCache;