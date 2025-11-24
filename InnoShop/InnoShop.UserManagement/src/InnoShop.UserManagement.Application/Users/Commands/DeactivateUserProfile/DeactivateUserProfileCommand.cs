using ErrorOr;
using InnoShop.UserManagement.Application.Common.Authorization;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUser;

[Authorize(Roles = "Admin")]
public record DeactivateUserProfileCommand(
    Guid UserId
) : IRequest<ErrorOr<Success>>;