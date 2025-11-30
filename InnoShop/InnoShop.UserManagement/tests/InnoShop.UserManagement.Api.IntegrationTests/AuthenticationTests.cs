using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InnoShop.UserManagement.Contracts.Authentication;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InnoShop.UserManagement.Api.IntegrationTests;

[Collection(nameof(AspireAppCollection))]
public class AuthenticationTests
{
    private readonly AspireAppFixture _fixture;

    public AuthenticationTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    private HttpClient ApiClient => _fixture.UsersApiClient;

    private async Task<UserManagementDbContext> CreateDbContextAsync()
    {
        var connectionString = await _fixture.App.GetConnectionStringAsync("innoshop-users");
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new UserManagementDbContext(options, null!, null!, null!);
    }

    [Fact]
    public async Task PostRegister_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var registerData = new { Email = "testuser@example.com", Password = "P@ssw0rd123" };

        // Act: POST /authentication/register
        using var response = await ApiClient.PostAsJsonAsync(
            "/authentication/register",
            registerData);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        content.Should().NotBeNull();
        content!.Email.Should().Be("testuser@example.com");

        // Verify in database
        await using var dbContext = await CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Email.Value == "testuser@example.com");

        user.Should().NotBeNull();
        user!.IsEmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task PostForgotPassword_WhenValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        var email = "forgotpassword@example.com";
        var password = "P@ssw0rd123";

        // First register a user
        var registerData = new { Email = email, Password = password };
        await ApiClient.PostAsJsonAsync("/authentication/register", registerData);

        // Act: POST /authentication/forgot-password
        var forgotPasswordData = new { Email = email };
        using var response = await ApiClient.PostAsJsonAsync(
            "/authentication/forgot-password",
            forgotPasswordData);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("reset link");

        // Verify token was generated in database
        await using var dbContext = await CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Email.Value == email);

        user.Should().NotBeNull();
        user!.PasswordResetToken.Should().NotBeNull();
        user.PasswordResetTokenExpiration.Should().NotBeNull();
    }

    [Fact]
    public async Task PostForgotPassword_WhenEmailNotFound_ShouldStillReturnSuccess()
    {
        // Arrange - Security best practice: don't reveal if email exists
        var forgotPasswordData = new { Email = "nonexistent@example.com" };

        // Act: POST /authentication/forgot-password
        using var response = await ApiClient.PostAsJsonAsync(
            "/authentication/forgot-password",
            forgotPasswordData);

        // Assert - Should return 200 OK even if email doesn't exist
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostResetPassword_WhenValidToken_ShouldResetPassword()
    {
        // Arrange
        var email = "resetpassword@example.com";
        var password = "P@ssw0rd123";
        var newPassword = "NewP@ssw0rd456";

        // Register a user
        var registerData = new { Email = email, Password = password };
        await ApiClient.PostAsJsonAsync("/authentication/register", registerData);

        // Request password reset
        var forgotPasswordData = new { Email = email };
        await ApiClient.PostAsJsonAsync("/authentication/forgot-password", forgotPasswordData);

        // Get token from database
        await using var dbContext = await CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Email.Value == email);
        var token = user!.PasswordResetToken!;

        // Act: POST /authentication/reset-password
        var resetPasswordData = new { Email = email, Token = token, NewPassword = newPassword };
        using var response = await ApiClient.PostAsJsonAsync(
            "/authentication/reset-password",
            resetPasswordData);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify token was cleared
        await dbContext.Entry(user).ReloadAsync();
        user.PasswordResetToken.Should().BeNull();
        user.PasswordResetTokenExpiration.Should().BeNull();
    }

    [Fact]
    public async Task PostResetPassword_WhenInvalidToken_ShouldReturnError()
    {
        // Arrange
        var email = "invalidtoken@example.com";
        var password = "P@ssw0rd123";

        // Register a user
        var registerData = new { Email = email, Password = password };
        await ApiClient.PostAsJsonAsync("/authentication/register", registerData);

        // Act: POST /authentication/reset-password with invalid token
        var resetPasswordData = new { Email = email, Token = "invalid-token", NewPassword = "NewPassword123!" };
        using var response = await ApiClient.PostAsJsonAsync(
            "/authentication/reset-password",
            resetPasswordData);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}