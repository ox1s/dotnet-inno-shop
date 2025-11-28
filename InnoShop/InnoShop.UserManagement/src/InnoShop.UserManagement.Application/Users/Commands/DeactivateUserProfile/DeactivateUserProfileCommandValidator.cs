using FluentValidation;
using InnoShop.UserManagement.Application.Users.Commands.DeactivateUserProfile;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUser;

public class DeactivateUserProfileCommandValidator : AbstractValidator<DeactivateUserProfileCommand>
{
    public DeactivateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}