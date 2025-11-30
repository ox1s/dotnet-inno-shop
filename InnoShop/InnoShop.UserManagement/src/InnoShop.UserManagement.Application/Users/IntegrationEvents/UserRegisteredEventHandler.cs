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

        if (user is null) return;

        if (user.IsEmailVerified) return;

        var verificationLink = linkFactory.Create(user.Id, user.EmailVerificationToken!);

        await emailSender.SendEmailAsync(
            user.Email.Value,
            "noreply@innoshop.com",
            "Welcome to InnoShop! Verify your email",
            $"Please verify: <a href='{verificationLink}'>Click here</a>"
        );
    }
}