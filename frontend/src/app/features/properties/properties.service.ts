import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Property, PropertyPayload } from '../../shared/models/property.models';

@Injectable({ providedIn: 'root' })
export class PropertiesService {
  constructor(private http: HttpClient) {}

  list() { return this.http.get<Property[]>(`${environment.apiBaseUrl}/properties`); }
  create(payload: PropertyPayload) { return this.http.post<Property>(`${environment.apiBaseUrl}/properties`, payload); }
  update(id: string, payload: PropertyPayload) { return this.http.put<Property>(`${environment.apiBaseUrl}/properties/${id}`, payload); }
  remove(id: string) { return this.http.delete<void>(`${environment.apiBaseUrl}/properties/${id}`); }
}
