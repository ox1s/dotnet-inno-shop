using ErrorOr;
using InnoShop.Users.Application.Authentication.Common;
using MediatR;

namespace InnoShop.Users.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;