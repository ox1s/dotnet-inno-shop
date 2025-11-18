using ErrorOr;
using InnoShop.Users.Domain.Common;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.Users.Events;

namespace InnoShop.Users.Domain.Users;

public class User : Entity
{
    private Username Username { get; } = null!;
    private Email Email { get; } = null!;
    private readonly string _passwordHash = null!;


    private Role role { get; } = null!;
    private bool IsActive { get; } = true;
    private bool IsEmailConfirmed { get; } = false;

    private Guid? ProfileId { get; }

    private User(
        Username username,
        Email email,
        string passwordHash,
        Role role,
        Guid? id = null)
            : base(id ?? Guid.NewGuid())
    {
        Username = username;
        Email = email;
        _passwordHash = passwordHash;
        role = Role.Customer;
        IsActive = true;

        _domainEvents.Add(new UserCreatedEvent(Id));
    }
    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.IsCorrectPassword(password, _passwordHash);
    }

    public static User CreateCustomer(
         Username username,
         Email email,
         string passwordHash)
         => new(username, email, passwordHash, Role.Customer);

    public static User CreateAdmin(
            Username username,
            Email email,
            string passwordHash)
            => new(username, email, passwordHash, Role.Admin);

    private User() { }
}
