import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PropertiesService } from '../properties/properties.service';
import { TenantsService } from './tenants.service';
import { Property } from '../../shared/models/property.models';
import { Tenant, TenantPayload } from '../../shared/models/tenant.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container grid">
    <h2>Tenants</h2>

    <div *ngIf="message" class="card" style="border-left:4px solid #2563eb;">{{message}}</div>
    <div *ngIf="error" class="card error">{{error}}</div>

    <div class="card grid">
      <select [(ngModel)]="selectedPropertyId" (change)="onPropertyChange()">
        <option value="">Select property</option>
        <option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option>
      </select>

      <div class="row" *ngIf="selectedPropertyId">
        <input [(ngModel)]="form.fullName" placeholder="Name" />
        <input [(ngModel)]="form.phone" placeholder="Phone" />
        <input [(ngModel)]="form.roomOrBed" placeholder="Room/Bed" />
        <input [(ngModel)]="form.monthlyRent" type="number" min="1" placeholder="Rent" />
        <input [(ngModel)]="form.rentDueDay" type="number" min="1" max="28" placeholder="Due day" />
        <button [disabled]="saving" (click)="addTenant()">{{saving ? 'Saving...' : 'Add Tenant'}}</button>
      </div>
      <div *ngIf="formError" class="error">{{formError}}</div>
    </div>

    <div class="card" *ngIf="selectedPropertyId">
      <table>
        <thead><tr><th>Name</th><th>Phone</th><th>Rent</th><th>Room</th><th>Due Day</th><th>Actions</th></tr></thead>
        <tbody>
          <tr *ngFor="let t of tenants">
            <ng-container *ngIf="editingTenantId !== t.id; else editRow">
              <td>{{t.fullName}}</td>
              <td>{{t.phone}}</td>
              <td>₹{{t.monthlyRent}}</td>
              <td>{{t.roomOrBed}}</td>
              <td>{{t.rentDueDay}}</td>
              <td>
                <div class="row">
                  <button [disabled]="saving" (click)="startEdit(t)">Edit</button>
                  <button [disabled]="saving" (click)="remove(t.id)">Delete</button>
                </div>
              </td>
            </ng-container>
            <ng-template #editRow>
              <td><input [(ngModel)]="editForm.fullName" placeholder="Name" /></td>
              <td><input [(ngModel)]="editForm.phone" placeholder="Phone" /></td>
              <td><input [(ngModel)]="editForm.monthlyRent" type="number" min="1" /></td>
              <td><input [(ngModel)]="editForm.roomOrBed" placeholder="Room/Bed" /></td>
              <td><input [(ngModel)]="editForm.rentDueDay" type="number" min="1" max="28" /></td>
              <td>
                <div class="row">
                  <button [disabled]="saving" (click)="updateTenant()">{{saving ? 'Saving...' : 'Update'}}</button>
                  <button [disabled]="saving" (click)="cancelEdit()">Cancel</button>
                </div>
              </td>
            </ng-template>
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
  saving = false;
  message = '';
  error = '';
  formError = '';

  editingTenantId: string | null = null;

  form: TenantPayload = { fullName: '', phone: '', roomOrBed: '', monthlyRent: 0, rentDueDay: 5, moveInDate: new Date().toISOString().slice(0, 10) };
  editForm: TenantPayload = { fullName: '', phone: '', roomOrBed: '', monthlyRent: 0, rentDueDay: 5, isActive: true };

  constructor(private propertyService: PropertiesService, private tenantService: TenantsService) {}

  ngOnInit() {
    this.propertyService.list().subscribe({
      next: (res) => this.properties = res,
      error: (e) => this.error = e?.error?.error ?? 'Failed to load properties'
    });
  }

  onPropertyChange() {
    this.clearMessages();
    this.cancelEdit();
    this.loadTenants();
  }

  loadTenants() {
    if (!this.selectedPropertyId) {
      this.tenants = [];
      return;
    }

    this.tenantService.listByProperty(this.selectedPropertyId).subscribe({
      next: (res) => this.tenants = res,
      error: (e) => this.error = e?.error?.error ?? 'Failed to load tenants'
    });
  }

  addTenant() {
    this.clearMessages();
    if (!this.selectedPropertyId) {
      this.formError = 'Select a property first.';
      return;
    }

    this.formError = this.validate(this.form, false);
    if (this.formError) return;

    this.saving = true;
    this.tenantService.create(this.selectedPropertyId, this.form).subscribe({
      next: () => {
        this.saving = false;
        this.form = { fullName: '', phone: '', roomOrBed: '', monthlyRent: 0, rentDueDay: 5, moveInDate: new Date().toISOString().slice(0, 10) };
        this.message = 'Tenant added successfully.';
        this.loadTenants();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to add tenant';
      }
    });
  }

  startEdit(tenant: Tenant) {
    this.clearMessages();
    this.editingTenantId = tenant.id;
    this.editForm = {
      fullName: tenant.fullName,
      phone: tenant.phone,
      roomOrBed: tenant.roomOrBed,
      monthlyRent: tenant.monthlyRent,
      rentDueDay: tenant.rentDueDay,
      email: tenant.email,
      isActive: tenant.isActive
    };
  }

  updateTenant() {
    if (!this.editingTenantId) return;

    this.clearMessages();
    this.formError = this.validate(this.editForm, true);
    if (this.formError) return;

    this.saving = true;
    this.tenantService.update(this.editingTenantId, this.editForm).subscribe({
      next: () => {
        this.saving = false;
        this.editingTenantId = null;
        this.message = 'Tenant updated successfully.';
        this.loadTenants();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to update tenant';
      }
    });
  }

  cancelEdit() {
    this.editingTenantId = null;
    this.formError = '';
  }

  remove(tenantId: string) {
    this.clearMessages();
    if (!confirm('Delete this tenant? This cannot be undone.')) return;

    this.saving = true;
    this.tenantService.remove(tenantId).subscribe({
      next: () => {
        this.saving = false;
        this.message = 'Tenant deleted successfully.';
        this.loadTenants();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to delete tenant';
      }
    });
  }

  private validate(payload: TenantPayload, isUpdate: boolean) {
    if (!payload.fullName?.trim()) return 'Tenant name is required.';
    if (!payload.phone?.trim()) return 'Phone is required.';
    if (!payload.roomOrBed?.trim()) return 'Room/Bed is required.';

    if (!payload.monthlyRent || payload.monthlyRent <= 0) return 'Monthly rent must be greater than 0.';
    if (!payload.rentDueDay || payload.rentDueDay < 1 || payload.rentDueDay > 28) return 'Due day must be between 1 and 28.';

    if (isUpdate && payload.isActive === undefined) return 'Active status is required for update.';

    return '';
  }

  private clearMessages() {
    this.message = '';
    this.error = '';
    this.formError = '';
  }
}
