import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PropertiesService } from '../properties/properties.service';
import { TenantsService } from './tenants.service';
import { Property } from '../../shared/models/property.models';
import { Tenant } from '../../shared/models/tenant.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container grid">
    <h2>Tenants</h2>

    <div class="card grid">
      <select [(ngModel)]="selectedPropertyId" (change)="loadTenants()">
        <option value="">Select property</option>
        <option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option>
      </select>

      <div class="row" *ngIf="selectedPropertyId">
        <input [(ngModel)]="form.fullName" placeholder="Name" />
        <input [(ngModel)]="form.phone" placeholder="Phone" />
        <input [(ngModel)]="form.roomOrBed" placeholder="Room/Bed" />
        <input [(ngModel)]="form.monthlyRent" type="number" placeholder="Rent" />
        <input [(ngModel)]="form.rentDueDay" type="number" placeholder="Due day" />
        <button (click)="addTenant()">Add Tenant</button>
      </div>
    </div>

    <div class="card" *ngIf="selectedPropertyId">
      <table>
        <thead><tr><th>Name</th><th>Phone</th><th>Rent</th><th>Room</th><th></th></tr></thead>
        <tbody>
          <tr *ngFor="let t of tenants">
            <td>{{t.fullName}}</td><td>{{t.phone}}</td><td>₹{{t.monthlyRent}}</td><td>{{t.roomOrBed}}</td>
            <td><button (click)="remove(t.id)">Delete</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class TenantListPage implements OnInit {
  properties: Property[] = [];
  tenants: Tenant[] = [];
  selectedPropertyId = '';
  form = { fullName: '', phone: '', roomOrBed: '', monthlyRent: 0, rentDueDay: 5, moveInDate: new Date().toISOString().slice(0, 10) };

  constructor(private propertyService: PropertiesService, private tenantService: TenantsService) {}

  ngOnInit() { this.propertyService.list().subscribe((res) => this.properties = res); }

  loadTenants() {
    if (!this.selectedPropertyId) return;
    this.tenantService.listByProperty(this.selectedPropertyId).subscribe((res) => this.tenants = res);
  }

  addTenant() {
    if (!this.selectedPropertyId) return;
    this.tenantService.create(this.selectedPropertyId, this.form).subscribe(() => {
      this.form = { fullName: '', phone: '', roomOrBed: '', monthlyRent: 0, rentDueDay: 5, moveInDate: new Date().toISOString().slice(0, 10) };
      this.loadTenants();
    });
  }

  remove(tenantId: string) { this.tenantService.remove(tenantId).subscribe(() => this.loadTenants()); }
}
