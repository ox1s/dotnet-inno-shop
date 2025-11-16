using UserManagement.Domain.UnitTests.TestUtils.Users;
using FluentAssertions;

namespace UserManagement.Domain.UnitTests;

public class UserProfileTests
{
    [Fact]
    public void AddMoreProfileThanOneForUser_ShouldFail()
    {
        // Arrange
        // Create user 1
        var user1 = UserFactory.CreateUser();

        // Act
        // Add userProfile 1
        var addUserProfile1Result = user1.CreateProfile();
        var addUserProfile2Result = user1.CreateProfile();

        // Assert
        // userProfile 2 adding failed
        addUserProfile1Result.IsError.Should().BeFalse();

        addUserProfile2Result.IsError.Should().BeTrue();

        addUserProfile2Result.IsError.Should().BeTrue();
        addUserProfile2Result.FirstError.Should().Be(UserProfileErrors.CannotHaveMoreProfilesThanOneForUser);
    }
}
