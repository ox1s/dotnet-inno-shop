using ErrorOr;
using InnoShop.Users.Application.Authentication.Common;
using MediatR;

namespace InnoShop.Users.Application.Authentication.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;
