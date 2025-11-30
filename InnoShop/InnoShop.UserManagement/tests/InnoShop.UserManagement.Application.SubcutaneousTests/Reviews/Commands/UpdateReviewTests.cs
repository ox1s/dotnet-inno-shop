using ErrorOr;
using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
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
        // --------------------------------------------------------------------------------

        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        // Arrange
        var authorEmail = Email.Create("author@test.com").Value;
        var author = UserFactory.CreateUserWithProfile(authorEmail);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var targetEmail = Email.Create("target@test.com").Value;
        var targetUser = UserFactory.CreateUserWithProfile(targetEmail);

        dbContext.Users.AddRange(author, targetUser);
        await dbContext.SaveChangesAsync();

        var createCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUser.Id,
            4,
            "Оке"
        );

        var createResult = await mediator.Send(createCommand);
        createResult.IsError.Should().BeFalse();
        var reviewId = createResult.Value.Id;

        // Act
        var updateCommand = ReviewCommandFactory.CreateUpdateReviewCommand(
            reviewId,
            mediatorFactory.DefaultUserId,
            5,
            "Умопропроай"
        );

        var updateResult = await mediator.Send(updateCommand);

        var getReviewQuery = ReviewQueryFactory.CreateGetReviewQuery(reviewId);
        var getReviewResult = await mediator.Send(getReviewQuery);

        // Assert
        updateResult.IsError.Should().BeFalse();
        getReviewResult.Value.Rating.Should().Be(5);
        getReviewResult.Value.Comment.Should().Be("Умопропроай");
    }

    [Fact]
    public async Task UpdateReview_WhenNotAuthor_ShouldReturnNotTheReviewAuthor()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        // Arrange
        var author1 = UserFactory.CreateUserWithProfile(Email.Create("a@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author1, Guid.NewGuid());

        var author2 = UserFactory.CreateUserWithProfile(Email.Create("b@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author2, mediatorFactory.DefaultUserId);

        var target = UserFactory.CreateUserWithProfile(Email.Create("t@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author2, Guid.NewGuid());

        dbContext.Users.AddRange(author1, author2, target);
        await dbContext.SaveChangesAsync();

        var createCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            target.Id,
            3,
            "Ok."
        );

        mediatorFactory.SetCurrentUser(author1.Id);
        var created = await mediator.Send(createCommand);
        created.IsError.Should().BeFalse();
        var reviewId = created.Value.Id;

        mediatorFactory.SetCurrentUser(author2.Id);

        // Act
        var updateCommand = ReviewCommandFactory.CreateUpdateReviewCommand(
            reviewId,
            rating: 4
        );

        var updateResult = await mediator.Send(updateCommand);

        // Assert
        updateResult.IsError.Should().BeTrue();
        updateResult.FirstError.Type.Should().Be(ErrorType.Forbidden);
    }
}