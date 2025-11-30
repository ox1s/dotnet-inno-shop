using FluentEmail.Core;
using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.EmailService;

public class EmailSender(
    IFluentEmail email,
    ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string to, string from, string subject, string body)
    {
        logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);

        var fluentEmail = email
            .To(to)
            .Subject(subject)
            .Body(body, true);

        if (!string.IsNullOrEmpty(from)) fluentEmail.SetFrom(from);

        var response = await fluentEmail.SendAsync();

        if (!response.Successful)
        {
            var errors = string.Join(", ", response.ErrorMessages);
            logger.LogError("Failed to send email. Errors: {Errors}", errors);
            throw new Exception($"Failed to send email: {errors}");
        }

        logger.LogInformation("Email sent successfully!");
    }
}