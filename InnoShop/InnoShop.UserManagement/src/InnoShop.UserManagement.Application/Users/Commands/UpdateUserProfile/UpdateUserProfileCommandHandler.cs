using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<UpdateUserProfileCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (user.UserProfile is null)
        {
            return UserErrors.UserMustCreateAUserProfile;
        }

        var countryInput = command.Country ?? user.UserProfile.Location.Country;

        var firstNameInput = command.FirstName ?? user.UserProfile.FirstName.Value;
        var firstNameResult = FirstName.Create(firstNameInput);
        if (firstNameResult.IsError) return firstNameResult.Errors;

        var lastNameInput = command.LastName ?? user.UserProfile.LastName.Value;
        var lastNameResult = LastName.Create(lastNameInput);
        if (lastNameResult.IsError) return lastNameResult.Errors;

        var avatarInput = command.AvatarUrl ?? user.UserProfile.AvatarUrl.Value;
        var avatarUrlResult = AvatarUrl.Create(avatarInput);
        if (avatarUrlResult.IsError) return avatarUrlResult.Errors;

        var phoneInput = command.PhoneNumber ?? user.UserProfile.PhoneNumber.Value;
        var phoneNumberResult = PhoneNumber.Create(phoneInput, countryInput);
        if (phoneNumberResult.IsError) return phoneNumberResult.Errors;

        var stateInput = command.State ?? user.UserProfile.Location.State;
        var cityInput = command.City ?? user.UserProfile.Location.City;
        var locationResult = Location.Create(countryInput, stateInput, cityInput);
        if (locationResult.IsError) return locationResult.Errors;

        var userProfileResult = UserProfile.Create(
            firstNameResult.Value,
            lastNameResult.Value,
            avatarUrlResult.Value,
            phoneNumberResult.Value,
            locationResult.Value);
        if (userProfileResult.IsError) return userProfileResult.Errors;

        var updateResult = user.UpdateUserProfile(userProfileResult.Value);
        if (updateResult.IsError) return updateResult.Errors;

        await _usersRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}

