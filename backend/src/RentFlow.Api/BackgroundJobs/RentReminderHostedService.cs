using Microsoft.Extensions.Options;
using RentFlow.Application.Interfaces;
using RentFlow.Infrastructure.Reminders;

namespace RentFlow.Api.BackgroundJobs;

public class RentReminderHostedService(
    IServiceProvider serviceProvider,
    IOptions<ReminderOptions> options,
    ILogger<RentReminderHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Rent reminder hosted service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IRentReminderService>();
                var result = await reminderService.ProcessDueRemindersAsync(stoppingToken);
                logger.LogInformation("Rent reminders processed. Checked={Checked}, Sent={Sent}, Failed={Failed}", result.CheckedRecords, result.SentCount, result.FailedCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rent reminder hosted service iteration failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(options.Value.IntervalMinutes), stoppingToken);
        }
    }
}
