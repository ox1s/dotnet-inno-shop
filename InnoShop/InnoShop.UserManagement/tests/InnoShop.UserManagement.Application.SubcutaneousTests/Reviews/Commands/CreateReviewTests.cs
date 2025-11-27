using System.Threading.Tasks;
using FluentAssertions;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using InnoShop.UserManagementTestCommon.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Reviews.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateReviewTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    [Fact]
    public async Task CreateReview_WhenValidCommand_ShouldCreateReview()
    {
        // Arrange
        var user = await CreateUser();

        var createReviewCommand = ReviewCommandFactory.CreateCreateReviewCommand();

        // Act
        var createReviewResult = await _mediator.Send(createReviewCommand);

        // Assert
        createReviewResult.IsError.Should().BeFalse();
        createReviewResult.Value.AuthorId.Should().Be(user.Id);
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(200)]
    public async Task CreateReview_WhenCommandContainsInvalidData_ShouldReturnValidationError(int reviewRating)
    {
        // Arrange
        var createReviewCommand = ReviewCommandFactory.CreateCreateReviewCommand(rating: reviewRating);

        // Act
        var result = await _mediator.Send(createReviewCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Rating");
    }

    private async Task<User> CreateUser()
    {
        var createUserRegisterCommand = UserProfileCommandFactory.CreateRegisterCommand();

        var result = await _mediator.Send(createUserRegisterCommand);

        result.IsError.Should().BeFalse();

        return result.Value.User;
    }
}
