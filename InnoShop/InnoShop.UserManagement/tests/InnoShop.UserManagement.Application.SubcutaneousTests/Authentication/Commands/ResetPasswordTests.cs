using FluentAssertions;
using InnoShop.UserManagement.Application.Authentication.Commands.ResetPassword;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Authentication.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class ResetPasswordTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task ResetPassword_WhenValidToken_ShouldResetPassword()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        var user = UserFactory.CreateUser(email: Email.Create("reset@example.com").Value);
        user.RequestPasswordReset();
        var token = user.PasswordResetToken!;
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Act
        var command = new ResetPasswordCommand("reset@example.com", token, "NewPassword123!");
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();

        var updatedUser = await dbContext.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.PasswordResetToken.Should().BeNull();
        updatedUser.PasswordResetTokenExpiration.Should().BeNull();
    }

    [Fact]
    public async Task ResetPassword_WhenInvalidToken_ShouldReturnError()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        var user = UserFactory.CreateUser(email: Email.Create("invalid@example.com").Value);
        user.RequestPasswordReset();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Act
        var command = new ResetPasswordCommand("invalid@example.com", "invalid-token", "NewPassword123!");
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("User.InvalidResetToken");
    }

    [Fact]
    public async Task ResetPassword_WhenUserNotFound_ShouldReturnError()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        await dbContext.SaveChangesAsync();

        // Act
        var command = new ResetPasswordCommand("notfound@example.com", "some-token", "NewPassword123!");
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.UserNotFound);
    }

    // Note: Token expiration testing is complex without time manipulation
    // and is better suited for integration tests with actual time delays
}

