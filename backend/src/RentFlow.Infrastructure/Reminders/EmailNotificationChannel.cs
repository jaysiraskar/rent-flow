using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Reminders;

public class EmailNotificationChannel(IOptions<SmtpOptions> smtpOptions) : INotificationChannel
{
    public string ChannelName => "Email";

    public async Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        var options = smtpOptions.Value;
        if (string.IsNullOrWhiteSpace(options.Host))
            throw new InvalidOperationException("SMTP host is not configured.");

        using var client = new SmtpClient(options.Host, options.Port)
        {
            Credentials = new NetworkCredential(options.Username, options.Password),
            EnableSsl = options.EnableSsl
        };

        using var message = new MailMessage
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
