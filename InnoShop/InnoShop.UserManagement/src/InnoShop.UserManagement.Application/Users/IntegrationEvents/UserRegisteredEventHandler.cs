using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Application.Common.Interfaces;
using MediatR;

namespace InnoShop.UserManagement.Application.Users.IntegrationEvents;

public class UserRegisteredEventHandler(
    IUsersRepository usersRepository,
    IEmailSender emailSender,
    IEmailVerificationLinkFactory linkFactory)
    : INotificationHandler<UserRegisteredIntegrationEvent>
{
    public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        if (user.IsEmailVerified)
        {
            return;
        }

        var verificationLink = linkFactory.Create(user.Id, user.EmailVerificationToken!);

        await emailSender.SendEmailAsync(
            to: user.Email.Value,
            from: "noreply@innoshop.com",
            subject: "Welcome to InnoShop! Verify your email",
            body: $"Please verify: <a href='{verificationLink}'>Click here</a>"
        );
    }
}