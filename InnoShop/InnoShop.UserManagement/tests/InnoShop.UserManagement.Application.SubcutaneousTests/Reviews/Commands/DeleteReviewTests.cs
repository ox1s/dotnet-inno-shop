using ErrorOr;
using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.ReviewAggregate;
using InnoShop.UserManagement.TestCommon.UserAggregate;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Reviews.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class DeleteReviewTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task DeleteReview_WhenAuthor_ShouldDeleteReview()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        // Arrange
        var author = UserFactory.CreateUserWithProfile(email: Email.Create("author@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var target = UserFactory.CreateUserWithProfile(email: Email.Create("target@test.com").Value);

        dbContext.Users.AddRange(author, target);
        await dbContext.SaveChangesAsync();

        // Act
        var createCommand = ReviewCommandFactory.CreateCreateReviewCommand(
            targetUserId: target.Id,
            rating: 4
        );

        var created = await mediator.Send(createCommand);
        created.IsError.Should().BeFalse();
        var reviewId = created.Value.Id;

        mediatorFactory.SetCurrentUser(
            author.Id,
            roles: [AppRoles.Seller],
            permissions: [AppPermissions.Review.Delete]
        );

        var deleteCommand = ReviewCommandFactory.CreateDeleteReviewCommand(
            userId: author.Id,
            reviewId: reviewId);

        var deletedResult = await mediator.Send(deleteCommand);

        deletedResult.IsError.Should().BeFalse();
        var dbReview = await dbContext.Reviews.FindAsync(reviewId);

        // Assert
        dbReview!.IsDeleted.Should().BeTrue();

        // Почему-то если тесты проходят вместе - FAILED, но в отдельности - OK. Почему?
    }

    [Fact]
    public async Task DeleteReview_WhenNotAuthor_ShouldReturnForbidden()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var authorA = UserFactory.CreateUserWithProfile(email: Email.Create("a@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(authorA, Guid.NewGuid());

        var authorB = UserFactory.CreateUserWithProfile(email: Email.Create("b@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(authorB, mediatorFactory.DefaultUserId);

        var target = UserFactory.CreateUserWithProfile(email: Email.Create("t@test.com").Value);

        dbContext.Users.AddRange(authorA, authorB, target);
        await dbContext.SaveChangesAsync();

        mediatorFactory.SetCurrentUser(authorA.Id);
        var created = await mediator.Send(ReviewCommandFactory.CreateCreateReviewCommand(targetUserId: target.Id, rating: 2));
        created.IsError.Should().BeFalse();
        var reviewId = created.Value.Id;

        mediatorFactory.SetCurrentUser(
            authorB.Id,
            roles: [AppRoles.Seller],
            permissions: [AppPermissions.Review.Delete]
        );
        var result = await mediator.Send(ReviewCommandFactory.CreateDeleteReviewCommand(reviewId: reviewId));

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Forbidden);

        var dbReview = await dbContext.Reviews.FindAsync(reviewId);
        dbReview.Should().NotBeNull();
        dbReview!.IsDeleted.Should().BeFalse();
    }
}