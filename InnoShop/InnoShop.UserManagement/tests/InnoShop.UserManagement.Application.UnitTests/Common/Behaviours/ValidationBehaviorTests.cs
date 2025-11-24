using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using InnoShop.UserManagement.Application.Common.Behaviours;
using InnoShop.UserManagement.Application.Users.Authentication.Common;
using InnoShop.UserManagement.Application.Users.Authentication.Commands.Register;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.TestConstants;


namespace InnoShop.UserManagement.Application.UnitTests.Common.Behaviours;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<RegisterCommand, ErrorOr<AuthenticationResult>> _validationBehavior;
    private readonly IValidator<RegisterCommand> _mockValidator;
    private readonly RequestHandlerDelegate<ErrorOr<AuthenticationResult>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        _mockValidator = Substitute.For<IValidator<RegisterCommand>>();
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<AuthenticationResult>>>();

        _validationBehavior = new ValidationBehavior<RegisterCommand, ErrorOr<AuthenticationResult>>(_mockValidator);
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var registerUserRequest = UserCommandFactory.CreateRegisterCommand();
        var user = User.CreateUser(
            Constants.User.Email,
            Constants.User.PasswordHash
        );

        var authResult = new AuthenticationResult(user, "jwt_token");

        _mockValidator
            .ValidateAsync(registerUserRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _mockNextBehavior.Invoke().Returns(authResult);

        // Act
        var result = await _validationBehavior.Handle(registerUserRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(authResult);

        await _mockNextBehavior.Received(1).Invoke();
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var registerUserRequest = new RegisterCommand("bad-email", "123");
        List<ValidationFailure> validationFailures = [new(propertyName: "foo", errorMessage: "bad foo")];

        _mockValidator
            .ValidateAsync(registerUserRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(registerUserRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("foo");
        result.FirstError.Description.Should().Be("bad foo");

        await _mockNextBehavior.DidNotReceive().Invoke();
    }
    [Fact]
    public async Task InvokeBehavior_WhenValidatorIsNull_ShouldInvokeNextBehavior()
    {

        var behaviorWithoutValidator = new ValidationBehavior<RegisterCommand, ErrorOr<AuthenticationResult>>(validator: null);

        var command = new RegisterCommand("any@test.com", "any");
        var authResult = new AuthenticationResult(
            User.CreateUser(Email.Create("a@b.com").Value, "h"), "t");

        _mockNextBehavior.Invoke().Returns((ErrorOr<AuthenticationResult>)authResult);

        // Act
        var result = await behaviorWithoutValidator.Handle(command, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockNextBehavior.Received(1).Invoke();
    }
}

