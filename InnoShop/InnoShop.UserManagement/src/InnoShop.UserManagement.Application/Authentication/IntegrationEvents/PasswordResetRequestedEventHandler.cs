using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using InnoShop.UserManagement.Application.Common.Interfaces;
using MediatR;

namespace InnoShop.UserManagement.Application.Authentication.IntegrationEvents;

public class PasswordResetRequestedEventHandler(
    IEmailSender emailSender,
    IEmailVerificationLinkFactory linkFactory)
    : INotificationHandler<PasswordResetRequestedIntegrationEvent>
{
    public async Task Handle(PasswordResetRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var resetLink = linkFactory.CreateResetPasswordLink(notification.Email, notification.Token);

        await emailSender.SendEmailAsync(
            notification.Email,
            "security@innoshop.com",
            "Reset Your Password",
            $@"
                <h3>Password Reset Request</h3>
                <p>You requested a password reset. Click the link below to set a new password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>");
    }
}