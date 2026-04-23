namespace RentFlow.Application.DTOs.Reminders;

public record ReminderDispatchResult(int CheckedRecords, int SentCount, int FailedCount);
