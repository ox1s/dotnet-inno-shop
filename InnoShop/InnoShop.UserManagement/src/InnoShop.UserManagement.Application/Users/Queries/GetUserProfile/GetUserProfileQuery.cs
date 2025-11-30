using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Users;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

[Authorize(Permissions = AppPermissions.UserProfile.Read)]
public record GetUserProfileQuery(Guid UserId) : IRequest<ErrorOr<UserProfileResponse>>;