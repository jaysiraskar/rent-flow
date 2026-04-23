import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { DashboardDueItem, DashboardSummary } from '../../shared/models/rent-record.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private http: HttpClient) {}

  getSummary(year: number, month: number, propertyId?: string) {
    const params = new URLSearchParams({ year: String(year), month: String(month) });
    if (propertyId) params.set('propertyId', propertyId);
    return this.http.get<DashboardSummary>(`${environment.apiBaseUrl}/dashboard/summary?${params.toString()}`);
  }

  getUpcomingDues(days = 7, propertyId?: string) {
    const params = new URLSearchParams({ days: String(days) });
    if (propertyId) params.set('propertyId', propertyId);
    return this.http.get<DashboardDueItem[]>(`${environment.apiBaseUrl}/dashboard/upcoming-dues?${params.toString()}`);
  }

  getOverdue(propertyId?: string) {
    const params = new URLSearchParams();
    if (propertyId) params.set('propertyId', propertyId);
    return this.http.get<DashboardDueItem[]>(`${environment.apiBaseUrl}/dashboard/overdue?${params.toString()}`);
  }
}
