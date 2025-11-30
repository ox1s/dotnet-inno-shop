using FluentValidation;

namespace InnoShop.UserManagement.Application.Authentication.Commands.CreateAdmin;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(5);
    }
}
