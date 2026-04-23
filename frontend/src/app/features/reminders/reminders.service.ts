import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ReminderDispatchResult, ReminderLog } from '../../shared/models/reminder.models';

@Injectable({ providedIn: 'root' })
export class RemindersService {
  constructor(private http: HttpClient) {}

  runNow() {
    return this.http.post<ReminderDispatchResult>(`${environment.apiBaseUrl}/reminders/run-now`, {});
  }

  logs(year?: number, month?: number, propertyId?: string, take = 100) {
    const params = new URLSearchParams({ take: String(take) });
    if (year) params.set('year', String(year));
    if (month) params.set('month', String(month));
    if (propertyId) params.set('propertyId', propertyId);
    return this.http.get<ReminderLog[]>(`${environment.apiBaseUrl}/reminders/logs?${params.toString()}`);
  }
}
