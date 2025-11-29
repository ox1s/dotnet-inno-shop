using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;

public class CreateUserProfileCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserProfileCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(CreateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var firstNameResult = FirstName.Create(command.FirstName);
        if (firstNameResult.IsError) return firstNameResult.Errors;

        var lastNameResult = LastName.Create(command.LastName);
        if (lastNameResult.IsError) return lastNameResult.Errors;

        var avatarUrlResult = AvatarUrl.Create(command.AvatarUrl);
        if (avatarUrlResult.IsError) return avatarUrlResult.Errors;

        var phoneNumberResult = PhoneNumber.Create(command.PhoneNumber, command.Country);
        if (phoneNumberResult.IsError) return phoneNumberResult.Errors;

        var locationResult = Location.Create(command.Country, command.State, command.City);
        if (locationResult.IsError) return locationResult.Errors;

        var createUserProfileResult = UserProfile.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            avatarUrlResult.Value,
            phoneNumberResult.Value,
            locationResult.Value
        );
        if (createUserProfileResult.IsError) return createUserProfileResult.Errors;

        var result = user.CreateUserProfile(createUserProfileResult.Value);
        if (result.IsError) return result.Errors;

        await usersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return user;
    }

}
