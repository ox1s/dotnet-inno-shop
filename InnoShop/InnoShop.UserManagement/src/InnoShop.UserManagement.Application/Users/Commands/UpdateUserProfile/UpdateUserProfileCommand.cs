using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;


using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Update, Policies = AppPolicies.SelfOrAdmin)]
public record UpdateUserProfileCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? PhoneNumber,
    Country? Country,
    string? State,
    string? City)
    : IAuthorizeableRequest<ErrorOr<Success>>;
