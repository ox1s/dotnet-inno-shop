using ErrorOr;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Authentication.Common;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.CreateAdmin;

public class CreateAdminCommandHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    IEmailVerificationLinkFactory linkFactory,
    ICurrentUserProvider currentUserProvider
)
    : IRequestHandler<CreateAdminCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(CreateAdminCommand command,
        CancellationToken cancellationToken)
    {
        // Check if any admin exists
        var adminExists = await usersRepository.AnyAdminExistsAsync(cancellationToken);
        
        // If admins exist, require current user to be an admin
        if (adminExists)
        {
            try
            {
                var currentUser = currentUserProvider.GetCurrentUser();
                if (!currentUser.Roles.Contains(AppRoles.Admin))
                    return Error.Forbidden(description: "Only existing admins can create new admin accounts.");
            }
            catch
            {
                return Error.Unauthorized(description: "Authentication required to create admin accounts when admins already exist.");
            }
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsError) return emailResult.Errors;

        var email = emailResult.Value;
        if (await usersRepository.ExistsByEmailAsync(email, cancellationToken))
            return Error.Conflict(description: "User already exists");

        var hashPasswordResult = passwordHasher.HashPassword(command.Password);
        if (hashPasswordResult.IsError) return hashPasswordResult.Errors;

        var user = User.CreateAdmin(
            email,
            hashPasswordResult.Value);

        await usersRepository.AddUserAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        var verificationLink = linkFactory.Create(user.Id, user.EmailVerificationToken!);

        await emailSender.SendEmailAsync(
            user.Email.Value,
            "noreply@innoshop.com",
            "Welcome to InnoShop Admin! Please verify your email.",
            $"Hello! Click here to verify: <a href='{verificationLink}'>Verify Email</a>"
        );

        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}
