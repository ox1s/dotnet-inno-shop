using ErrorOr;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;
public record CreateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string AvatarUrl,
    string PhoneNumber,
    Country Country,
    string State,
    string City)
    : IRequest<ErrorOr<User>>;