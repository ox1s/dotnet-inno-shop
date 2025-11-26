using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using InnoShop.UserManagement.Application.Common.Behaviours;
using InnoShop.UserManagement.Application.Authentication.Common;
using InnoShop.UserManagement.Application.Authentication.Commands.Register;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagementTestCommon.ReviewAggregate;
using InnoShop.UserManagement.TestCommon.ReviewAggregate;


namespace InnoShop.UserManagement.Application.UnitTests.Common.Behaviours;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateReviewCommand, ErrorOr<Review>> _validationBehavior;
    private readonly IValidator<CreateReviewCommand> _mockValidator;
    private readonly RequestHandlerDelegate<ErrorOr<Review>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        _mockValidator = Substitute.For<IValidator<CreateReviewCommand>>();
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Review>>>();
        _validationBehavior = new ValidationBehavior<CreateReviewCommand, ErrorOr<Review>>(_mockValidator);
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createReviewRequest = ReviewCommandFactory.CreateCreateReviewCommand();
        var review = ReviewFactory.CreateReview();

        _mockValidator
            .ValidateAsync(createReviewRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _mockNextBehavior.Invoke().Returns(review);

        // Act
        var result = await _validationBehavior.Handle(createReviewRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(review);
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var createReviewRequest = ReviewCommandFactory.CreateCreateReviewCommand();
        List<ValidationFailure> validationFailures = [new(propertyName: "бяка", errorMessage: "бяка случилась")];

        _mockValidator
            .ValidateAsync(createReviewRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(createReviewRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("бяка");
        result.FirstError.Description.Should().Be("бяка случилась");
    }
    [Fact]
    public async Task InvokeBehavior_WhenValidatorIsNull_ShouldInvokeNextBehavior()
    {
        // Arrange
        var behaviorWithoutValidator = new ValidationBehavior<CreateReviewCommand, ErrorOr<Review>>(validator: null);

        var createReviewRequest = ReviewCommandFactory.CreateCreateReviewCommand();
        var review = ReviewFactory.CreateReview();

        _mockNextBehavior.Invoke().Returns((ErrorOr<Review>)review);

        // Act
        var result = await behaviorWithoutValidator.Handle(createReviewRequest, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockNextBehavior.Received(1).Invoke();
    }
}

