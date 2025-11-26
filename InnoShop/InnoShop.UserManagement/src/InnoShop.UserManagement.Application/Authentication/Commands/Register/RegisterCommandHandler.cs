using ErrorOr;
using InnoShop.UserManagement.Application.Authentication.Common;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IJwtTokenGenerator _jwtTokenGenerator,
    IPasswordHasher _passwordHasher,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IEmailSender _emailSender,
     IEmailVerificationLinkFactory _linkFactory
    )
        : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        var email = emailResult.Value;
        if (await _usersRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Error.Conflict(description: "User already exists");
        }

        var hashPasswordResult = _passwordHasher.HashPassword(command.Password);
        if (hashPasswordResult.IsError)
        {
            return hashPasswordResult.Errors;
        }

        var user = User.CreateUser(
            email,
            hashPasswordResult.Value);


        await _usersRepository.AddUserAsync(user, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        var verificationLink = _linkFactory.Create(user.Id, user.EmailVerificationToken!);


        await _emailSender.SendEmailAsync(
            to: user.Email.Value,
            from: "noreply@innoshop.com",
            subject: "Welcome to InnoShop! Please verify your email.",
            body: $"Hello! Click here to verify: <a href='{verificationLink}'>Verify Email</a>"
        );

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthenticationResult(user, token);
    }
}