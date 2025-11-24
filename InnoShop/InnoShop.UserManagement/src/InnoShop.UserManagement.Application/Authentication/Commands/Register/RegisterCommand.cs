using ErrorOr;
using InnoShop.UserManagement.Application.Authentication.Common;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;