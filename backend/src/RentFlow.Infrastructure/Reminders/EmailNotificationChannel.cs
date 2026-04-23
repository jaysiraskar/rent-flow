using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Reminders;

public class EmailNotificationChannel(IOptions<SmtpOptions> smtpOptions, ILogger<EmailNotificationChannel> logger) : INotificationChannel
{
    public string ChannelName => "Email";

    public async Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        SmtpOptions options = smtpOptions.Value;
        if (string.IsNullOrWhiteSpace(options.Host))
        {
            logger.LogWarning("SMTP host not configured. Skipping email send to {Recipient}", recipient);
            return;
        }

        using SmtpClient client = new(options.Host, options.Port)
        {
            Credentials = new NetworkCredential(options.Username, options.Password),
            EnableSsl = options.EnableSsl
        };

        using MailMessage message = new()
        {
            From = new MailAddress(options.FromEmail, options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(recipient);
        await client.SendMailAsync(message, cancellationToken);
    }
}
