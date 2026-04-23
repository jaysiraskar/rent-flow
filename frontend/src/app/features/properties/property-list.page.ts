import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PropertiesService } from './properties.service';
import { Property, PropertyPayload } from '../../shared/models/property.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
  <div class="container grid">
    <div class="row justify-between">
      <h2>Properties</h2>
      <button [disabled]="saving || loading" (click)="toggleCreateForm()">{{showForm ? 'Cancel' : 'Add Property'}}</button>
    </div>

    <div *ngIf="loading" class="card">Loading properties...</div>
    <div *ngIf="message" class="card" style="border-left:4px solid #2563eb;">{{message}}</div>
    <div *ngIf="error" class="card error">{{error}}</div>

    <div class="card" *ngIf="showForm">
      <div class="grid">
        <input [(ngModel)]="form.name" placeholder="Property name" />
        <input [(ngModel)]="form.addressLine1" placeholder="Address" />
        <div class="row">
          <input [(ngModel)]="form.city" placeholder="City" />
          <input [(ngModel)]="form.state" placeholder="State" />
          <input [(ngModel)]="form.pincode" placeholder="Pincode" />
        </div>
        <div *ngIf="formError" class="error">{{formError}}</div>
        <button [disabled]="saving || !canSubmitForm(form)" (click)="save()">{{saving ? 'Saving...' : 'Save'}}</button>
      </div>
    </div>

    <div class="card" *ngIf="!loading">
      <table>
        <thead><tr><th>Name</th><th>Location</th><th>Actions</th></tr></thead>
        <tbody>
          <tr *ngFor="let p of properties">
            <td>
              <ng-container *ngIf="editingPropertyId !== p.id; else editName">
                <a [routerLink]="['/properties', p.id]">{{p.name}}</a>
              </ng-container>
              <ng-template #editName><input [(ngModel)]="editForm.name" placeholder="Property name" /></ng-template>
            </td>
            <td>
              <ng-container *ngIf="editingPropertyId !== p.id; else editLocation">
                {{p.city}}, {{p.state}}
              </ng-container>
              <ng-template #editLocation>
                <div class="grid">
                  <input [(ngModel)]="editForm.addressLine1" placeholder="Address" />
                  <div class="row">
                    <input [(ngModel)]="editForm.city" placeholder="City" />
                    <input [(ngModel)]="editForm.state" placeholder="State" />
                    <input [(ngModel)]="editForm.pincode" placeholder="Pincode" />
                  </div>
                </div>
              </ng-template>
            </td>
            <td>
              <div class="row" *ngIf="editingPropertyId !== p.id">
                <button [disabled]="saving" (click)="startEdit(p)">Edit</button>
                <button [disabled]="saving" (click)="remove(p.id)">Delete</button>
              </div>
              <div class="row" *ngIf="editingPropertyId === p.id">
                <button [disabled]="saving || !canSubmitForm(editForm)" (click)="update()">{{saving ? 'Saving...' : 'Update'}}</button>
                <button [disabled]="saving" (click)="cancelEdit()">Cancel</button>
              </div>
            </td>
          </tr>
          <tr *ngIf="properties.length === 0"><td colspan="3" class="muted">No properties yet.</td></tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class PropertyListPage implements OnInit {
  properties: Property[] = [];
  showForm = false;
  loading = false;
  saving = false;
  message = '';
  error = '';
  formError = '';

  editingPropertyId: string | null = null;

  form: PropertyPayload = { name: '', addressLine1: '', city: '', state: '', pincode: '' };
  editForm: PropertyPayload = { name: '', addressLine1: '', city: '', state: '', pincode: '' };

  constructor(private propertiesService: PropertiesService) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.error = '';

    this.propertiesService.list().subscribe({
      next: (res) => {
        this.properties = res;
        this.loading = false;
      },
      error: (e) => {
        this.error = e?.error?.error ?? 'Failed to load properties';
        this.loading = false;
      }
    });
  }

  toggleCreateForm() {
    this.clearMessages();
    this.showForm = !this.showForm;
    if (!this.showForm) this.form = { name: '', addressLine1: '', city: '', state: '', pincode: '' };
  }

  save() {
    this.clearMessages();
    this.formError = this.validate(this.form);
    if (this.formError) return;

    this.saving = true;
    this.propertiesService.create(this.form).subscribe({
      next: () => {
        this.saving = false;
        this.showForm = false;
        this.form = { name: '', addressLine1: '', city: '', state: '', pincode: '' };
        this.message = 'Property created successfully.';
        this.load();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to create property';
      }
    });
  }

  startEdit(property: Property) {
    this.clearMessages();
    this.editingPropertyId = property.id;
    this.editForm = {
      name: property.name,
      addressLine1: property.addressLine1,
      city: property.city,
      state: property.state,
      pincode: property.pincode
    };
  }

  cancelEdit() {
    this.editingPropertyId = null;
    this.formError = '';
  }

  update() {
    if (!this.editingPropertyId) return;
    this.clearMessages();
    this.formError = this.validate(this.editForm);
    if (this.formError) return;

    this.saving = true;
    this.propertiesService.update(this.editingPropertyId, this.editForm).subscribe({
      next: () => {
        this.saving = false;
        this.editingPropertyId = null;
        this.message = 'Property updated successfully.';
        this.load();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to update property';
      }
    });
  }

  remove(id: string) {
    this.clearMessages();
    if (!confirm('Delete this property? This cannot be undone.')) return;

    this.saving = true;
    this.propertiesService.remove(id).subscribe({
      next: () => {
        this.saving = false;
        this.message = 'Property deleted successfully.';
        this.load();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to delete property';
      }
    });
  }

  canSubmitForm(payload: PropertyPayload) {
    return !this.validate(payload);
  }

  private validate(payload: PropertyPayload) {
    if (!payload.name.trim()) return 'Property name is required.';
    if (!payload.addressLine1.trim()) return 'Address is required.';
    if (!payload.city.trim()) return 'City is required.';
    if (!payload.state.trim()) return 'State is required.';
    if (!payload.pincode.trim()) return 'Pincode is required.';
    return '';
  }

  private clearMessages() {
    this.error = '';
    this.message = '';
    this.formError = '';
  }
}