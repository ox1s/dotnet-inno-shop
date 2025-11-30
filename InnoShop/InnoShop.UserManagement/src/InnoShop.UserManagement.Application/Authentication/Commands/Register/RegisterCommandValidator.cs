using FluentValidation;

namespace InnoShop.UserManagement.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(5);
    }
}