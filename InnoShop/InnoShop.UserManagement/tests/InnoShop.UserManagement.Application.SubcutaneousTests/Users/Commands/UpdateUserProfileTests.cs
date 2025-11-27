using FluentAssertions;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Users.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class UpdateUserProfileTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task UpdateProfile_WhenValid_ShouldUpdateProfile()
    {
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        var user = UserFactory.CreateUserWithProfile(email: Email.Create("u@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(user, mediatorFactory.DefaultUserId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var command = UserProfileCommandFactory.CreateUpdateUserProfileCommand(mediatorFactory.DefaultUserId);

        var result = await mediator.Send(command);

        result.IsError.Should().BeFalse();

        var dbUser = await dbContext.Users.FindAsync(mediatorFactory.DefaultUserId);
        
        dbUser.UserProfile.FirstName.Should().Be("Updated");
        dbUser.UserProfile.LastName.Should().Be("Name");
        dbUser.UserProfile.AvatarUrl.Should().Be("https://example.com/avatar.jpg");
        dbUser.UserProfile.PhoneNumber.Should().Be("+123456789");
        dbUser.UserProfile.Location.Country.Should().Be(Country.Belarus);
        dbUser.UserProfile.Location.State.Should().Be("Minsk");
        dbUser.UserProfile.Location.City.Should().Be("Minsk");           
    }
}