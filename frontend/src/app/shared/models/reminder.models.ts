export interface ReminderDispatchResult {
  checkedRecords: number;
  sentCount: number;
  failedCount: number;
  processedAtUtc: string;
}

export interface ReminderLog {
  id: string;
  rentRecordId: string;
  tenantId: string;
  propertyId: string;
  propertyName: string;
  tenantName: string;
  channel: string;
  reminderType: string;
  recipient: string;
  success: boolean;
  failureReason?: string;
  sentAtUtc: string;
}