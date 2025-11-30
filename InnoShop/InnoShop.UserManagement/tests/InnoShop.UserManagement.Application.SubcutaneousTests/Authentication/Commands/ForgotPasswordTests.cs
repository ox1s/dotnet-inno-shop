using FluentAssertions;
using InnoShop.UserManagement.Application.Authentication.Commands.ForgotPassword;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Authentication.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class ForgotPasswordTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task ForgotPassword_WhenValidEmail_ShouldGenerateResetToken()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        dbContext.AttachRange(Role.List);
        var user = UserFactory.CreateUser(Email.Create("test@example.com").Value);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Act
        var command = new ForgotPasswordCommand("test@example.com");
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();

        var updatedUser = await dbContext.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.PasswordResetToken.Should().NotBeNull();
        updatedUser.PasswordResetTokenExpiration.Should().NotBeNull();
    }

    [Fact]
    public async Task ForgotPassword_WhenEmailNotFound_ShouldReturnSuccess()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        await dbContext.SaveChangesAsync();

        // Act
        var command = new ForgotPasswordCommand("nonexistent@example.com");
        var result = await mediator.Send(command);

        // Assert - Should return success to not reveal email existence (security best practice)
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task ForgotPassword_WhenRateLimitExceeded_ShouldReturnError()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        dbContext.AttachRange(Role.List);
        var user = UserFactory.CreateUser(Email.Create("ratelimit@example.com").Value);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Set rate limit to 3 attempts
        var cacheKey = "reset-password-limit:ratelimit@example.com";
        await cache.SetStringAsync(cacheKey, "3", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });

        // Act
        var command = new ForgotPasswordCommand("ratelimit@example.com");
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("RateLimit.Exceeded");
    }

    [Fact]
    public async Task ForgotPassword_ShouldIncrementRateLimitCounter()
    {
        // Arrange
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        dbContext.AttachRange(Role.List);
        var user = UserFactory.CreateUser(Email.Create("counter@example.com").Value);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Act - First request
        var command = new ForgotPasswordCommand("counter@example.com");
        await mediator.Send(command);

        // Assert
        var cacheKey = "reset-password-limit:counter@example.com";
        var attempts = await cache.GetStringAsync(cacheKey);
        attempts.Should().Be("1");

        // Act - Second request
        await mediator.Send(command);

        // Assert
        attempts = await cache.GetStringAsync(cacheKey);
        attempts.Should().Be("2");
    }
}