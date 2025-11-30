using ErrorOr;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<ErrorOr<Success>>;

