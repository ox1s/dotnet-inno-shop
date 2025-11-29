using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.ReviewAggregate;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using InnoShop.UserManagementTestCommon.ReviewAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Reviews.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetReviewsTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task GetReviews_ForTargetUser_ShouldReturnReviews()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var author = UserFactory.CreateUserWithProfile(email: Email.Create("a@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(author, mediatorFactory.DefaultUserId);

        var target = UserFactory.CreateUserWithProfile(email: Email.Create("t@test.com").Value);

        dbContext.Users.AddRange(author, target);
        await dbContext.SaveChangesAsync();

        var result1 = await mediator.Send(ReviewCommandFactory.CreateCreateReviewCommand(targetUserId: target.Id, rating: 5, comment: "123"));

        result1.IsError.Should().BeFalse();
        result1.Value.Should().NotBeNull();

        var query = ReviewQueryFactory.CreateGetReviewQuery(result1.Value.Id);
        var resultQuery = await mediator.Send(query);

        resultQuery.IsError.Should().BeFalse();
        resultQuery.Value.Should().NotBeNull();
        resultQuery.Value!.TargetUserId.Should().Be(target.Id);
    }
}