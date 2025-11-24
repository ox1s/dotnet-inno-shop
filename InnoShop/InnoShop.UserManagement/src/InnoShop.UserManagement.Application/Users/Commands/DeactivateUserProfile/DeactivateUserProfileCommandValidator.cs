using FluentValidation;

namespace InnoShop.UserManagement.Application.Users.Commands.DeactivateUser;

public class DeactivateUserProfileCommandValidator : AbstractValidator<DeactivateUserProfileCommand>
{
    public DeactivateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}