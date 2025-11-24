using FluentValidation;

namespace InnoShop.UserManagement.Application.Users.Commands.ActivateUser;

public class ActivateUserProfileCommandValidator : AbstractValidator<ActivateUserProfileCommand>
{
    public ActivateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}