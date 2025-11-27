using ErrorOr;
using InnoShop.UserManagement.Application.Common.Security.Request;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.ActivateUser;

[Authorize(Roles = "Admin")]
public record ActivateUserProfileCommand(
    Guid UserId
) : IRequest<ErrorOr<Success>>;