using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.TestConstants;
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
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var user = UserFactory.CreateUserWithProfile(Email.Create("u@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(user, mediatorFactory.DefaultUserId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var command = UserProfileCommandFactory.CreateUpdateUserProfileCommand(
            mediatorFactory.DefaultUserId);

        var result = await mediator.Send(command);

        result.IsError.Should().BeFalse();

        var dbUser = await dbContext.Users.FindAsync(mediatorFactory.DefaultUserId);

        dbUser.Should().NotBeNull();
        dbUser!.UserProfile.Should().NotBeNull();
        dbUser.UserProfile.FirstName.Value.Should().Be(Constants.UserProfile.FirstName.Value);
        dbUser.UserProfile.LastName.Value.Should().Be(Constants.UserProfile.LastName.Value);
        dbUser.UserProfile.AvatarUrl.Value.Should().Be(Constants.UserProfile.AvatarUrl.Value);
        dbUser.UserProfile.PhoneNumber.Value.Should().Be(Constants.UserProfile.ValidPhoneNumberBelarus.Value);
        dbUser.UserProfile.Location.Country.Value.Should().Be(Constants.UserProfile.ValidLocationBelarus.Country.Value);
        dbUser.UserProfile.Location.State.Should().Be(Constants.UserProfile.ValidLocationBelarus.State);
        dbUser.UserProfile.Location.City.Should().Be(Constants.UserProfile.ValidLocationBelarus.City);
    }
}