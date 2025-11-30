using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Users.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetUserProfileTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task GetUserProfile_WhenExists_ShouldReturnProfile()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var user = UserFactory.CreateUserWithProfile(Email.Create("get@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(user, mediatorFactory.DefaultUserId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var query = UserQueryFactory.CreateGetUserProfileQuery(mediatorFactory.DefaultUserId);
        var result = await mediator.Send(query);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(mediatorFactory.DefaultUserId);
    }
}