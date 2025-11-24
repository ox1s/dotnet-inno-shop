using ErrorOr;
using InnoShop.UserManagement.Application.Users.Authentication.Common;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Authentication.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;
