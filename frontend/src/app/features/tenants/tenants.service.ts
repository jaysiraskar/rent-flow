import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Tenant, TenantPayload } from '../../shared/models/tenant.models';

@Injectable({ providedIn: 'root' })
export class TenantsService {
  constructor(private http: HttpClient) {}

  listByProperty(propertyId: string) { return this.http.get<Tenant[]>(`${environment.apiBaseUrl}/properties/${propertyId}/tenants`); }
  create(propertyId: string, payload: TenantPayload) { return this.http.post<Tenant>(`${environment.apiBaseUrl}/properties/${propertyId}/tenants`, payload); }
  update(tenantId: string, payload: TenantPayload) { return this.http.put<Tenant>(`${environment.apiBaseUrl}/tenants/${tenantId}`, payload); }
  remove(tenantId: string) { return this.http.delete<void>(`${environment.apiBaseUrl}/tenants/${tenantId}`); }
}
