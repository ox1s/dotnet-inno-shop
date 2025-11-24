using ErrorOr;
using InnoShop.UserManagement.Application.Users.Authentication.Common;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;