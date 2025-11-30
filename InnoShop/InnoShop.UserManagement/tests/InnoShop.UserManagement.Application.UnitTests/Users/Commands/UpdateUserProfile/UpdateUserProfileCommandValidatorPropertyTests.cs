using FluentValidation.TestHelper;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.UnitTests.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidatorPropertyTests
{
    private readonly UpdateUserProfileCommandValidator _validator = new();

    [Property(MaxTest = 100)]
    public Property FirstName_TooShort_ShouldFailValidation()
    {
        var shortNameGen = ArbMap.Default.GeneratorFor<string>()
            .Where(s => s != null && s.Length < 2);

        return Prop.ForAll(shortNameGen.ToArbitrary(), shortName =>
        {
            var command = CreateValidCommand() with
            {
                FirstName = shortName
            };
            var result = _validator.TestValidate(command);

            return result.ShouldHaveValidationErrorFor(x => x.FirstName) != null;
        });
    }

    [Property(MaxTest = 100)]
    public Property FirstName_TooLong_ShouldFailValidation()
    {
        var longNameGen = Gen.Choose(51, 100)
            .Select(len => new string('A', len));

        return Prop.ForAll(longNameGen.ToArbitrary(), longName =>
        {
            var command = CreateValidCommand() with
            {
                FirstName = longName
            };
            var result = _validator.TestValidate(command);

            return result.ShouldHaveValidationErrorFor(x => x.FirstName) != null;
        });
    }

    [Property(MaxTest = 100)]
    public Property LastName_TooShort_ShouldFailValidation()
    {
        var shortNameGen = ArbMap.Default.GeneratorFor<string>()
            .Where(s => s != null && s.Length < 2);

        return Prop.ForAll(shortNameGen.ToArbitrary(), shortName =>
        {
            var command = CreateValidCommand() with
            {
                LastName = shortName
            };
            var result = _validator.TestValidate(command);

            return result.ShouldHaveValidationErrorFor(x => x.LastName) != null;
        });
    }

    [Property(MaxTest = 100)]
    public Property LastName_TooLong_ShouldFailValidation()
    {
        var longNameGen = Gen.Choose(51, 100)
            .Select(len => new string('B', len));

        return Prop.ForAll(longNameGen.ToArbitrary(), longName =>
        {
            var command = CreateValidCommand() with
            {
                LastName = longName
            };
            var result = _validator.TestValidate(command);
            return result.ShouldHaveValidationErrorFor(x => x.LastName) != null;
        });
    }

    [Property(MaxTest = 100)]
    public Property ValidNames_ShouldPassValidation()
    {
        var validChars = new[] { 'A', 'B', 'C', 'a', 'b', 'c', 'А', 'Б', 'а', 'б' };
        var validNameGen = Gen.Choose(2, 50)
            .SelectMany(len => Gen.Elements(validChars).ArrayOf(len)
                .Select(chars => new string(chars)));

        return Prop.ForAll(validNameGen.ToArbitrary(), validName =>
        {
            var command = CreateValidCommand() with
            {
                FirstName = validName,
                LastName = validName
            };
            var result = _validator.TestValidate(command);

            return !result.Errors.Any(e => e.PropertyName == "FirstName" || e.PropertyName == "LastName");
        });
    }

    [Property(MaxTest = 100)]
    public Property DisallowedCountry_ShouldFailValidation()
    {
        var disallowedCountries = Country.List
            .Where(c => !Country.AllowedCountries.Contains(c))
            .ToArray();

        if (disallowedCountries.Length == 0)
            return true.ToProperty();

        var disallowedCountryGen = Gen.Elements(disallowedCountries);

        return Prop.ForAll(disallowedCountryGen.ToArbitrary(), disallowedCountry =>
        {
            var command = CreateValidCommand() with
            {
                Country = disallowedCountry
            };
            var result = _validator.TestValidate(command);
            return result.ShouldHaveValidationErrorFor(x => x.Country) != null;
        });
    }

    [Property(MaxTest = 100)]
    public Property AllowedCountry_ShouldPassValidation()
    {
        var allowedCountryGen = Gen.Elements(Country.AllowedCountries.ToArray());

        return Prop.ForAll(allowedCountryGen.ToArbitrary(), allowedCountry =>
        {
            var command = CreateValidCommand() with
            {
                Country = allowedCountry
            };

            var result = _validator.TestValidate(command);

            return !result.Errors.Any(e => e.PropertyName == "Country");
        });
    }

    private static UpdateUserProfileCommand CreateValidCommand()
    {
        return new UpdateUserProfileCommand(
            Guid.NewGuid(),
            "John",
            "Doe",
            "https://example.com/avatar.jpg",
            "+375291234567",
            Country.Belarus,
            "Minsk",
            "Minsk");
    }
}