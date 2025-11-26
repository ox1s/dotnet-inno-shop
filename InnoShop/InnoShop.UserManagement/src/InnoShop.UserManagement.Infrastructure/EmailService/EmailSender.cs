using FluentEmail.Core;
using InnoShop.UserManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace InnoShop.UserManagement.Infrastructure.EmailService;

public class EmailSender(
    IFluentEmail _email,
    ILogger<EmailSender> _logger) : IEmailSender
{
    public async Task SendEmailAsync(string to, string from, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);

        var email = _email
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: true);

        if (!string.IsNullOrEmpty(from))
        {
            email.SetFrom(from);
        }

        var response = await email.SendAsync();

        if (!response.Successful)
        {
            var errors = string.Join(", ", response.ErrorMessages);
            _logger.LogError("Failed to send email. Errors: {Errors}", errors);
            throw new Exception($"Failed to send email: {errors}");
        }

        _logger.LogInformation("Email sent successfully!");
    }
}