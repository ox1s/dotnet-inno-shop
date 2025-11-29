using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.VerifyEmail;

[Authorize(Permissions = AppRoles.Registered)]
public record VerifyEmailCommand(
    Guid UserId,
    string Token) : IRequest<ErrorOr<Success>>, IInvalidatesUserCache;