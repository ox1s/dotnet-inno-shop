using ErrorOr;
using InnoShop.UserManagement.Application.Authentication.Common;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
     IEmailVerificationLinkFactory linkFactory
    )
        : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsError) return emailResult.Errors;

        var email = emailResult.Value;
        if (await usersRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Error.Conflict(description: "User already exists");
        }

        var hashPasswordResult = passwordHasher.HashPassword(command.Password);
        if (hashPasswordResult.IsError) return hashPasswordResult.Errors;


        var user = User.CreateUser(
            email,
            hashPasswordResult.Value);


        await usersRepository.AddUserAsync(user, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        var verificationLink = linkFactory.Create(user.Id, user.EmailVerificationToken!);


        await emailSender.SendEmailAsync(
            to: user.Email.Value,
            from: "noreply@innoshop.com",
            subject: "Welcome to InnoShop! Please verify your email.",
            body: $"Hello! Click here to verify: <a href='{verificationLink}'>Verify Email</a>"
        );

        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }
}