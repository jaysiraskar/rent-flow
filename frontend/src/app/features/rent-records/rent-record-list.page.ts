import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RentRecordsService } from './rent-records.service';
import { PropertiesService } from '../properties/properties.service';
import { Property } from '../../shared/models/property.models';
import { RentRecord } from '../../shared/models/rent-record.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container grid">
    <h2>Rent Records</h2>

    <div *ngIf="message" class="card" style="border-left:4px solid #2563eb;">{{message}}</div>
    <div *ngIf="error" class="card error">{{error}}</div>

    <div class="card row">
      <input type="number" [(ngModel)]="year" placeholder="Year" style="max-width:120px" />
      <input type="number" [(ngModel)]="month" placeholder="Month" style="max-width:120px" />
      <select [(ngModel)]="propertyId"><option value="">All properties</option><option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option></select>
      <select [(ngModel)]="status"><option value="">All status</option><option>Unpaid</option><option>Partial</option><option>Paid</option></select>
      <button [disabled]="loading || saving" (click)="load()">Filter</button>
      <button [disabled]="loading || saving" (click)="generate()">{{saving ? 'Generating...' : 'Generate Monthly'}}</button>
    </div>

    <div *ngIf="loading" class="card">Loading rent records...</div>

    <div class="card" *ngIf="!loading">
      <table>
        <thead><tr><th>Tenant</th><th>Property</th><th>Due</th><th>Expected</th><th>Paid</th><th>Status</th><th>Payment Update</th><th></th></tr></thead>
        <tbody>
          <tr *ngFor="let r of records">
            <td>{{r.tenantName}}</td>
            <td>{{r.propertyName}}</td>
            <td>{{r.dueDate}}</td>
            <td>₹{{r.expectedAmount}}</td>
            <td>₹{{r.paidAmount}}</td>
            <td>{{r.status}}</td>
            <td>
              <div class="row">
                <input
                  type="number"
                  min="0"
                  [max]="r.expectedAmount"
                  style="max-width:120px"
                  [(ngModel)]="paymentDrafts[r.id].paidAmount"
                  placeholder="Paid amount" />
                <input style="min-width:180px" [(ngModel)]="paymentDrafts[r.id].notes" placeholder="Notes (optional)" />
              </div>
            </td>
            <td>
              <button [disabled]="saving || !canSubmitPayment(r)" (click)="updatePayment(r)">
                {{saving ? 'Saving...' : 'Save Payment'}}
              </button>
            </td>
          </tr>
          <tr *ngIf="records.length === 0"><td colspan="8" class="muted">No records for the selected filters.</td></tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class RentRecordListPage implements OnInit {
  year = new Date().getFullYear();
  month = new Date().getMonth() + 1;
  propertyId = '';
  status = '';

  loading = false;
  saving = false;
  message = '';
  error = '';

  properties: Property[] = [];
  records: RentRecord[] = [];
  paymentDrafts: Record<string, { paidAmount: number; notes: string }> = {};

  constructor(private recordsService: RentRecordsService, private propertiesService: PropertiesService) {}

  ngOnInit() {
    this.propertiesService.list().subscribe({
      next: (res) => this.properties = res,
      error: (e) => this.error = e?.error?.error ?? 'Failed to load properties'
    });

    this.load();
  }

  load() {
    this.clearMessages();
    this.loading = true;

    this.recordsService.list(this.year, this.month, this.propertyId || undefined, this.status || undefined).subscribe({
      next: (res) => {
        this.loading = false;
        this.records = res;
        this.paymentDrafts = {};
        for (const record of res) {
          this.paymentDrafts[record.id] = { paidAmount: record.paidAmount, notes: record.notes ?? '' };
        }
      },
      error: (e) => {
        this.loading = false;
        this.error = e?.error?.error ?? 'Failed to load rent records';
      }
    });
  }

  generate() {
    this.clearMessages();
    this.saving = true;

    this.recordsService.generateMonthly(this.year, this.month, this.propertyId || undefined).subscribe({
      next: (res) => {
        this.saving = false;
        this.message = `Generated ${res.generated} monthly record(s).`;
        this.load();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to generate monthly records';
      }
    });
  }

  canSubmitPayment(record: RentRecord) {
    const draft = this.paymentDrafts[record.id];
    if (!draft) return false;
    if (draft.paidAmount < 0) return false;
    if (draft.paidAmount > record.expectedAmount) return false;
    return draft.paidAmount !== record.paidAmount || (draft.notes ?? '') !== (record.notes ?? '');
  }

  updatePayment(record: RentRecord) {
    const draft = this.paymentDrafts[record.id];
    if (!draft) return;

    if (draft.paidAmount < 0 || draft.paidAmount > record.expectedAmount) {
      this.error = `Paid amount for ${record.tenantName} must be between 0 and ${record.expectedAmount}.`;
      return;
    }

    this.clearMessages();
    this.saving = true;

    this.recordsService.updatePayment(record.id, draft.paidAmount, draft.notes?.trim() || undefined).subscribe({
      next: () => {
        this.saving = false;
        this.message = `Payment updated for ${record.tenantName}.`;
        this.load();
      },
      error: (e) => {
        this.saving = false;
        this.error = e?.error?.error ?? 'Failed to update payment';
      }
    });
  }

  private clearMessages() {
    this.message = '';
    this.error = '';
  }
}
