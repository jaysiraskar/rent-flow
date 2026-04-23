namespace RentFlow.Infrastructure.Reminders;

public class ReminderOptions
{
    public int UpcomingDaysThreshold { get; set; } = 3;
    public int IntervalMinutes { get; set; } = 60;
}
