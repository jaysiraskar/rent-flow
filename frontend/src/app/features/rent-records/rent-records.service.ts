import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { RentRecord } from '../../shared/models/rent-record.models';

@Injectable({ providedIn: 'root' })
export class RentRecordsService {
  constructor(private http: HttpClient) {}

  generateMonthly(year: number, month: number, propertyId?: string) {
    const params = new URLSearchParams({ year: String(year), month: String(month) });
    if (propertyId) params.set('propertyId', propertyId);
    return this.http.post<{ generated: number }>(`${environment.apiBaseUrl}/rent-records/generate-monthly?${params.toString()}`, {});
  }

  list(year: number, month: number, propertyId?: string, status?: string) {
    const params = new URLSearchParams({ year: String(year), month: String(month) });
    if (propertyId) params.set('propertyId', propertyId);
    if (status) params.set('status', status);
    return this.http.get<RentRecord[]>(`${environment.apiBaseUrl}/rent-records?${params.toString()}`);
  }

  updatePayment(id: string, paidAmount: number, notes?: string) {
    return this.http.put<RentRecord>(`${environment.apiBaseUrl}/rent-records/${id}/payment`, { paidAmount, notes });
  }
}
