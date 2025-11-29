using FluentAssertions;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using InnoShop.UserManagementTestCommon.ReviewAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.TestCommon.ReviewAggregate;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Reviews.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateReviewTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task CreateReview_WhenValidCommand_ShouldCreateReview()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        // Arrange
        var authorEmail = Email.Create("author@test.com").Value;
        var author = UserFactory.CreateUserWithProfile(email: authorEmail);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var targetEmail = Email.Create("target@test.com").Value;
        var targetUser = UserFactory.CreateUserWithProfile(email: targetEmail);

        dbContext.Users.AddRange(author, targetUser);
        await dbContext.SaveChangesAsync();

        var createReviewCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUserId: targetUser.Id
        );

        // Act
        var result = await mediator.Send(createReviewCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AuthorId.Should().Be(mediatorFactory.DefaultUserId);
        result.Value.TargetUserId.Should().Be(targetUser.Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    public async Task CreateReview_WhenInvalidRating_ShouldReturnValidationError(int invalidRating)
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        // Arrange
        var authorEmail = Email.Create("author@test.com").Value;
        var author = UserFactory.CreateUserWithProfile(email: authorEmail);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var targetEmail = Email.Create("target@test.com").Value;
        var targetUser = UserFactory.CreateUserWithProfile(email: targetEmail);
        typeof(Entity).GetProperty("Id")!.SetValue(targetUser, Guid.NewGuid());

        dbContext.Users.AddRange(author, targetUser);
        await dbContext.SaveChangesAsync();

        var command = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUserId: targetUser.Id,
            rating: invalidRating
        );

        // Act
        var result = await mediator.Send(command);


        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Rating.InvalidRange");
    }
}