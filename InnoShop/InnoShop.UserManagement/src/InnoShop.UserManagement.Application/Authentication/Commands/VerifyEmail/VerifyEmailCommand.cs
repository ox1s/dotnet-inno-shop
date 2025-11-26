using ErrorOr;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.VerifyEmail;

public record VerifyEmailCommand(
    Guid UserId,
    string Token) : IRequest<ErrorOr<Success>>;