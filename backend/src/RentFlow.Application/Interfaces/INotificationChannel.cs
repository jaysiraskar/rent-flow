namespace RentFlow.Application.Interfaces;

public interface INotificationChannel
{
    string ChannelName { get; }
    Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);
}
