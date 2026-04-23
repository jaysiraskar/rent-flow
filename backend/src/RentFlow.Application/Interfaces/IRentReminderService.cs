using RentFlow.Application.DTOs.Reminders;

namespace RentFlow.Application.Interfaces;

public interface IRentReminderService
{
    Task<ReminderDispatchResult> ProcessDueRemindersAsync(CancellationToken cancellationToken = default);
}
