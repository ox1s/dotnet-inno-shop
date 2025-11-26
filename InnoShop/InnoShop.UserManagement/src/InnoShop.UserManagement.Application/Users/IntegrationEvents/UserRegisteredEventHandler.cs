using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Application.Common.Interfaces;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.IntegrationEvents;

public class UserRegisteredEventHandler(
    IUsersRepository _usersRepository,
    IEmailSender _emailSender,
    IEmailVerificationLinkFactory _linkFactory)
    : INotificationHandler<UserRegisteredIntegrationEvent>
{
    public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        if (user.IsEmailVerified)
        {
            return;
        }

        var verificationLink = _linkFactory.Create(user.Id, user.EmailVerificationToken!);

        await _emailSender.SendEmailAsync(
            to: user.Email.Value,
            from: "noreply@innoshop.com",
            subject: "Welcome to InnoShop! Verify your email",
            body: $"Please verify: <a href='{verificationLink}'>Click here</a>"
        );
    }
}