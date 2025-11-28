using FluentAssertions;
using InnoShop.UserManagement.Application.SubcutaneousTests.Common;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Infrastructure.Persistence;
using InnoShop.UserManagement.TestCommon.UserAggregate;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.UserManagement.Application.SubcutaneousTests.Users.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateUserTests(MediatorFactory mediatorFactory)
{
    [Fact]
    public async Task CreateUser_WhenValidCommand_ShouldCreateUser()
    {
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------


        var command = UserCommandFactory.CreateCreateUserCommand(
            email: "newuser@test.com",
            password: "P@ssw0rd!");

        var result = await mediator.Send(command);

        result.IsError.Should().BeFalse();
        var createdId = result.Value.User.Id;
        var dbUser = await dbContext.Users.FindAsync(createdId);
        dbUser.Should().NotBeNull();
        dbUser!.Email.Value.Should().Be("newuser@test.com");
    }

    [Fact]
    public async Task CreateUser_WhenDuplicateEmail_ShouldReturnValidationError()
    {
        mediatorFactory.ResetDatabase();

        using var scope = mediatorFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

        dbContext.AttachRange(Role.List);
        // --------------------------------------------------------------------------------


        var existing = UserFactory.CreateUserWithProfile(email: Email.Create("dup@test.com").Value);
        dbContext.Users.Add(existing);
        await dbContext.SaveChangesAsync();

        var command = UserCommandFactory.CreateCreateUserCommand(
            email: "dup@test.com",
            password: "P@ssw0rd!"
        );

        var result = await mediator.Send(command);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code.Contains("Email") || e.Code.Contains("Conflict"));
    }
}