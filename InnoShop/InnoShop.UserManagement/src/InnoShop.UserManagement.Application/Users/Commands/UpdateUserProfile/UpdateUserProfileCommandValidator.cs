using FluentValidation;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ\- ]+$")
            .WithMessage("FirstName can only contain letters, hyphens, and spaces")
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ\- ]+$")
            .WithMessage("LastName can only contain letters, hyphens, and spaces")
            .When(x => x.LastName is not null);

        RuleFor(x => x.AvatarUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl))
            .WithMessage("AvatarUrl must be a valid HTTP or HTTPS URL");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .When(x => x.PhoneNumber != null);

        RuleFor(x => x.Country)
            .Must(BeAllowedCountry)
            .When(x => x.Country is not null)
            .WithMessage("Country must be in the allowed countries list");

        RuleFor(x => x.State)
            .NotEmpty()
            .When(x => x.State is not null);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        if (url.Length > 2048)
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == "http" || uri.Scheme == "https");
    }

    private static bool BeAllowedCountry(Country? country)
    {
        return country is not null && Country.AllowedCountries.Contains(country);
    }
}