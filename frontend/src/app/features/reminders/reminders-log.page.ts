import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PropertiesService } from '../properties/properties.service';
import { RemindersService } from './reminders.service';
import { Property } from '../../shared/models/property.models';
import { ReminderDispatchResult, ReminderLog } from '../../shared/models/reminder.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container grid">
    <h2>Reminder Logs</h2>

    <div *ngIf="message" class="card" style="border-left:4px solid #2563eb;">{{message}}</div>
    <div *ngIf="error" class="card error">{{error}}</div>

    <div class="card row">
      <input type="number" [(ngModel)]="year" placeholder="Year" style="max-width:120px" />
      <input type="number" [(ngModel)]="month" placeholder="Month" style="max-width:120px" />
      <select [(ngModel)]="propertyId">
        <option value="">All properties</option>
        <option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option>
      </select>
      <input type="number" min="1" max="500" [(ngModel)]="take" placeholder="Rows" style="max-width:120px" />
      <button [disabled]="loading || running" (click)="load()">{{loading ? 'Loading...' : 'Filter'}}</button>
      <button [disabled]="loading || running" (click)="runNow()">{{running ? 'Running...' : 'Run now'}}</button>
    </div>

    <div *ngIf="result" class="card">
      Last run: <strong>{{result.processedAtUtc}}</strong> · Checked: <strong>{{result.checkedRecords}}</strong>, Sent: <strong>{{result.sentCount}}</strong>, Failed: <strong>{{result.failedCount}}</strong>
    </div>

    <div class="card">
      <table>
        <thead><tr><th>Sent At</th><th>Property</th><th>Tenant</th><th>Type</th><th>Channel</th><th>Recipient</th><th>Status</th></tr></thead>
        <tbody>
          <tr *ngFor="let log of logs">
            <td>{{log.sentAtUtc}}</td>
            <td>{{log.propertyName}}</td>
            <td>{{log.tenantName}}</td>
            <td>{{log.reminderType}}</td>
            <td>{{log.channel}}</td>
            <td>{{log.recipient}}</td>
            <td>{{log.success ? 'Success' : ('Failed: ' + (log.failureReason || 'Unknown'))}}</td>
          </tr>
          <tr *ngIf="logs.length === 0"><td colspan="7" class="muted">No reminder logs for selected filters.</td></tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class ReminderLogPage implements OnInit {
  year = new Date().getFullYear();
  month = new Date().getMonth() + 1;
  take = 100;
  propertyId = '';
  properties: Property[] = [];
  logs: ReminderLog[] = [];
  result?: ReminderDispatchResult;
  loading = false;
  running = false;
  message = '';
  error = '';

  constructor(private remindersService: RemindersService, private propertiesService: PropertiesService) {}

  ngOnInit() {
    this.propertiesService.list().subscribe((res) => this.properties = res);
    this.load();
  }

  load() {
    this.clearMessages();
    this.loading = true;
    this.remindersService.logs(this.year, this.month, this.propertyId || undefined, this.take).subscribe({
      next: (res) => {
        this.loading = false;
        this.logs = res;
      },
      error: (e) => {
        this.loading = false;
        this.error = e?.error?.error ?? 'Failed to load reminder logs';
      }
    });
  }

  runNow() {
    this.clearMessages();
    this.running = true;

    this.remindersService.runNow().subscribe({
      next: (res) => {
        this.running = false;
        this.result = res;
        this.message = `Reminders run completed. Sent ${res.sentCount}, failed ${res.failedCount}.`;
        this.load();
      },
      error: (e) => {
        this.running = false;
        this.error = e?.error?.error ?? 'Failed to run reminders';
      }
    });
  }

  private clearMessages() {
    this.message = '';
    this.error = '';
  }
}
