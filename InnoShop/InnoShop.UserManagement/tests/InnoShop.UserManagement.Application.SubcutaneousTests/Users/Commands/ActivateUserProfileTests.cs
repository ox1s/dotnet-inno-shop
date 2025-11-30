using FluentAssertions;
using InnoShop.SharedKernel.Common;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Application.Users.Commands.ActivateUserProfile;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Users.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class ActivateUserProfileTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task ActivateUserProfile_WhenValid_ShouldActivateUser()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var user = UserFactory.CreateUserWithProfile(email: Email.Create("u@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(user, mediatorFactory.DefaultUserId);
        user.DeactivateUser();

        var rolesField = typeof(User).GetField("_roles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var roles = (List<Role>)rolesField!.GetValue(user)!;
        roles.Add(Role.Admin);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        mediatorFactory.SetCurrentUser(
            user.Id,
            roles: [AppRoles.Admin],
            permissions: [AppPermissions.UserProfile.Activate]
        );

        var command = UserProfileCommandFactory.CreateActivateUserProfileCommand(
            userId: user.Id);

        var result = await mediator.Send(command);

        result.IsError.Should().BeFalse();

        var dbUser = await dbContext.Users.FindAsync(mediatorFactory.DefaultUserId);

        dbUser.Should().NotBeNull();
        dbUser!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ActivateUserProfile_WhenUserNotFound_ShouldReturnError()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var adminUser = UserFactory.CreateUserWithProfile(email: Email.Create("admin@test.com").Value);
        var adminUserId = Guid.NewGuid();
        typeof(Entity).GetProperty("Id")!.SetValue(adminUser, adminUserId);

        var rolesField = typeof(User).GetField("_roles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var roles = (List<Role>)rolesField!.GetValue(adminUser)!;
        roles.Add(Role.Admin);

        dbContext.Users.Add(adminUser);
        await dbContext.SaveChangesAsync();

        mediatorFactory.SetCurrentUser(
            adminUserId,
            roles: [AppRoles.Admin],
            permissions: [AppPermissions.UserProfile.Activate]
        );

        var nonExistentUserId = Guid.NewGuid();
        var command = UserProfileCommandFactory.CreateActivateUserProfileCommand(
            userId: nonExistentUserId);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e == UserErrors.UserNotFound);
    }

    [Fact]
    public async Task ActivateUserProfile_WhenAlreadyActivated_ShouldReturnError()
    {
        // --------------------------------------------------------------------------------
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------

        var user = UserFactory.CreateUserWithProfile(email: Email.Create("u@test.com").Value);
        typeof(Entity).GetProperty("Id")!.SetValue(user, mediatorFactory.DefaultUserId);

        var rolesField = typeof(User).GetField("_roles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var roles = (List<Role>)rolesField!.GetValue(user)!;
        roles.Add(Role.Admin);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        mediatorFactory.SetCurrentUser(
            user.Id,
            roles: [AppRoles.Admin],
            permissions: [AppPermissions.UserProfile.Activate]
        );

        var command = UserProfileCommandFactory.CreateActivateUserProfileCommand(
            userId: user.Id);

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e == UserErrors.UserAlreadyActivated);
    }
}
