using ErrorOr;
using InnoShop.UserManagement.Application.Common.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security.Request;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;

[Authorize(Policies = Policy.SelfOrAdmin)]
public record UpdateUserProfileCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? PhoneNumber,
    Country? Country,
    string? State,
    string? City)
    : IRequest<ErrorOr<Success>>;
