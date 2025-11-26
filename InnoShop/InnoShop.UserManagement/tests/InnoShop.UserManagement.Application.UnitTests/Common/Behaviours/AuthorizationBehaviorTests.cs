using ErrorOr;
using FluentAssertions;
using InnoShop.UserManagement.Application.Common.Authorization;
using InnoShop.UserManagement.Application.Common.Behaviours;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Models;
using MediatR;
using NSubstitute;

namespace InnoShop.UserManagement.Application.UnitTests.Common.Behaviours;

public class AuthorizationBehaviorTests
{
    private readonly ICurrentUserProvider _mockCurrentUserProvider;
    private readonly RequestHandlerDelegate<ErrorOr<Success>> _mockNextBehavior;

    public AuthorizationBehaviorTests()
    {
        _mockCurrentUserProvider = Substitute.For<ICurrentUserProvider>();
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Success>>>();
    }

    [Fact]
    public async Task Handle_WhenRequestHasNoAttribute_ShouldInvokeNext()
    {
        // Arrange
        var request = new PublicCommand();
        var behavior = new AuthorizationBehavior<PublicCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        _mockNextBehavior.Invoke().Returns(Result.Success);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockNextBehavior.Received(1).Invoke();
        _mockCurrentUserProvider.DidNotReceive().GetCurrentUser();
    }

    [Fact]
    public async Task Handle_WhenUserHasRequiredRole_ShouldInvokeNext()
    {
        // Arrange
        var request = new AdminCommand();
        var behavior = new AuthorizationBehavior<AdminCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        var currentUser = new CurrentUser(
            Id: Guid.NewGuid(),
            Permissions: [],
            Roles: ["Admin"]
        );

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);
        _mockNextBehavior.Invoke().Returns(Result.Success);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockNextBehavior.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenUserMissingRequiredRole_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new AdminCommand();
        var behavior = new AuthorizationBehavior<AdminCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        var currentUser = new CurrentUser(
            Id: Guid.NewGuid(),
            Permissions: [],
            Roles: ["User"]
        );

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);
        result.FirstError.Description.Should().Be("User is forbidden from taking this action");

        await _mockNextBehavior.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WhenUserHasRequiredPermission_ShouldInvokeNext()
    {
        // Arrange
        var request = new ProtectedResourceCommand();
        var behavior = new AuthorizationBehavior<ProtectedResourceCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        var currentUser = new CurrentUser(
            Id: Guid.NewGuid(),
            Permissions: ["ReadResource"],
            Roles: []
        );

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);
        _mockNextBehavior.Invoke().Returns(Result.Success);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeFalse();
        await _mockNextBehavior.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WhenUserMissingRequiredPermission_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new ProtectedResourceCommand();
        var behavior = new AuthorizationBehavior<ProtectedResourceCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        var currentUser = new CurrentUser(
            Id: Guid.NewGuid(),
            Permissions: ["WriteResource"],
            Roles: []
        );

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);

        await _mockNextBehavior.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WhenUserHasMixedRequirements_AndMissingOne_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new ComplexCommand();
        var behavior = new AuthorizationBehavior<ComplexCommand, ErrorOr<Success>>(_mockCurrentUserProvider);

        var currentUser = new CurrentUser(
            Id: Guid.NewGuid(),
            Permissions: ["Delete"],
            Roles: ["User"]
        );

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);
        _mockNextBehavior.Invoke().Returns(Result.Success);

        // Act
        var result = await behavior.Handle(request, _mockNextBehavior, default);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);

        await _mockNextBehavior.DidNotReceive().Invoke();
    }


    public record PublicCommand : IRequest<ErrorOr<Success>>;

    [Authorize(Roles = "Admin")]
    public record AdminCommand : IRequest<ErrorOr<Success>>;

    [Authorize(Permissions = "ReadResource")]
    public record ProtectedResourceCommand : IRequest<ErrorOr<Success>>;

    [Authorize(Roles = "Admin", Permissions = "Delete")]
    public record ComplexCommand : IRequest<ErrorOr<Success>>;
}