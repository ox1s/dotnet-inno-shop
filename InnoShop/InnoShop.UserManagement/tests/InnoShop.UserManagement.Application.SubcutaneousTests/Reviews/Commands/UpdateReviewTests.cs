using FluentAssertions;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.ReviewAggregate;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using InnoShop.UserManagementTestCommon.ReviewAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Reviews.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class UpdateReviewTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task UpdateReview_WhenValidCommand_ShouldUpdateReview()
    {
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        // Arrange
        var authorEmail = Email.Create("author@test.com").Value;
        var author = UserFactory.CreateUserWithProfile(email: authorEmail);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var targetEmail = Email.Create("target@test.com").Value;
        var targetUser = UserFactory.CreateUserWithProfile(email: targetEmail);

        dbContext.Users.AddRange(author, targetUser);
        await dbContext.SaveChangesAsync();

        var createCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUserId: targetUser.Id,
            rating: 4,
            comment: "Good"
        );

        var createResult = await mediator.Send(createCommand);
        createResult.IsError.Should().BeFalse();
        var reviewId = createResult.Value.Id;

        // Act
        var updateCommand = ReviewCommandFactory.CreateUpdateReviewCommand(
            reviewId: reviewId,
            rating: 5,
            comment: "Excellent"
        );

        var updateResult = await mediator.Send(updateCommand);
        var getReviewQuery = ReviewQueryFactory.CreateGetReviewQuery(reviewId: reviewId);
        var getReviewResult = await mediator.Send(getReviewQuery);
        
        // Assert
        updateResult.IsError.Should().BeFalse();
        getReviewResult.Value.Rating.Value.Should().Be(5);
        getReviewResult.Value.Comment?.Value.Should().Be("Excellent");
    }

    [Fact]
    public async Task UpdateReview_WhenNotAuthor_ShouldReturnForbidden()
    {
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        // Arrange
        var author1 = UserFactory.CreateUserWithProfile(email: Email.Create("a@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author1, Guid.NewGuid());

        var author2 = UserFactory.CreateUserWithProfile(email: Email.Create("b@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author2, mediatorFactory.DefaultUserId);

        var target = UserFactory.CreateUserWithProfile(email: Email.Create("t@test.com").Value);

        dbContext.Users.AddRange(author1, author2, target);
        await dbContext.SaveChangesAsync();

        var createCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUserId: target.Id,
            rating: 3,
            comment: "Ok"
        );

        mediatorFactory.SetCurrentUserId(author1.Id);
        var created = await mediator.Send(createCommand);
        created.IsError.Should().BeFalse();
        var reviewId = created.Value.Id;

        mediatorFactory.SetCurrentUserId(author2.Id);

        // Act
        var updateCommand = ReviewCommandFactory.CreateUpdateReviewCommand(
            reviewId: reviewId,
            rating: 4
        );

        var updateResult = await mediator.Send(updateCommand);

        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.Errors.Should().Contain(e => e.Code.Contains("Forbidden") || e.Code.Contains("NotAuthor"));
    }
}