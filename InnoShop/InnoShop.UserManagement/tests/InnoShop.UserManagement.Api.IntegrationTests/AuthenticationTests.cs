using System.Net.Http.Json;
using FluentAssertions;
using InnoShop.UserManagement.Api.IntegrationTests.Common;
using InnoShop.UserManagement.Contracts.Authentication;
using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.UserManagement.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class AuthenticationTests
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public AuthenticationTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;

        _factory.ResetDatabase();
    }

    [Fact]
    public async Task PostRegister_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var registerRequest = new RegisterRequest("testuser@example.com", "P@ssw0rd123");

        // Act
        var response = await _client.PostAsJsonAsync("/authentication/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Email.Should().Be(registerRequest.Email);
        authResponse.Token.Should().NotBeNullOrEmpty();
        authResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnToken()
    {
        // Arrange
        var email = "loginuser@example.com";
        var password = "P@ssw0rd123";
        await _client.PostAsJsonAsync("/authentication/register", new RegisterRequest(email, password));

        var loginRequest = new LoginRequest(email, password);

        // Act
        var response = await _client.PostAsJsonAsync("/authentication/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        authResponse!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task PostForgotPassword_WhenValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        var email = "forgotpassword@example.com";
        var password = "P@ssw0rd123";

        await _client.PostAsJsonAsync("/authentication/register", new RegisterRequest(email, password));

        // Act: POST /authentication/forgot-password
        var forgotPasswordData = new { Email = email };
        using var response = await _client.PostAsJsonAsync(
            "/authentication/forgot-password",
            forgotPasswordData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("reset link");

        await using var dbContext = _factory.CreateDbContext();
        var emailVO = Email.Create(email).Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailVO);

        user.Should().NotBeNull();
        user!.PasswordResetToken.Should().NotBeNull();
        user.PasswordResetTokenExpiration.Should().NotBeNull();
    }

    [Fact]
    public async Task PostForgotPassword_WhenEmailNotFound_ShouldStillReturnSuccess()
    {
        // Arrange
        var forgotPasswordData = new { Email = "nonexistent@example.com" };

        // Act: POST /authentication/forgot-password
        using var response = await _client.PostAsJsonAsync(
            "/authentication/forgot-password",
            forgotPasswordData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostResetPassword_WhenValidToken_ShouldResetPassword()
    {
        // Arrange
        var email = "resetpassword@example.com";
        var password = "P@ssw0rd123";
        var newPassword = "NewP@ssw0rd456";

        // Register a user
        await _client.PostAsJsonAsync("/authentication/register", new RegisterRequest(email, password));

        // Request password reset
        var forgotPasswordData = new { Email = email };
        await _client.PostAsJsonAsync("/authentication/forgot-password", forgotPasswordData);

        // Get token from database
        await using var dbContext = _factory.CreateDbContext();

        var emailVO = Email.Create(email).Value;
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailVO);

        var token = user!.PasswordResetToken!;

        // Act: POST /authentication/reset-password
        var resetPasswordData = new { Email = email, Token = token, NewPassword = newPassword };
        using var response = await _client.PostAsJsonAsync(
            "/authentication/reset-password",
            resetPasswordData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

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
        await _client.PostAsJsonAsync("/authentication/register", new RegisterRequest(email, password));

        // Act: POST /authentication/reset-password with invalid token
        var resetPasswordData = new { Email = email, Token = "invalid-token", NewPassword = "NewPassword123!" };
        using var response = await _client.PostAsJsonAsync(
            "/authentication/reset-password",
            resetPasswordData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}