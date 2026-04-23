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

    <div class="card row">
      <input type="number" [(ngModel)]="year" placeholder="Year" style="max-width:120px" />
      <input type="number" [(ngModel)]="month" placeholder="Month" style="max-width:120px" />
      <select [(ngModel)]="propertyId">
        <option value="">All properties</option>
        <option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option>
      </select>
      <button (click)="load()">Filter</button>
      <button (click)="runNow()">Run now</button>
    </div>

    <div *ngIf="result" class="card">
      Checked: <strong>{{result.checkedRecords}}</strong>, Sent: <strong>{{result.sentCount}}</strong>, Failed: <strong>{{result.failedCount}}</strong>
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
        </tbody>
      </table>
    </div>
  </div>`
})
export class ReminderLogPage implements OnInit {
  year = new Date().getFullYear();
  month = new Date().getMonth() + 1;
  propertyId = '';
  properties: Property[] = [];
  logs: ReminderLog[] = [];
  result?: ReminderDispatchResult;

  constructor(private remindersService: RemindersService, private propertiesService: PropertiesService) {}

  ngOnInit() {
    this.propertiesService.list().subscribe((res) => this.properties = res);
    this.load();
  }

  load() {
    this.remindersService.logs(this.year, this.month, this.propertyId || undefined).subscribe((res) => this.logs = res);
  }

  runNow() {
    this.remindersService.runNow().subscribe((res) => {
      this.result = res;
      this.load();
    });
  }
}
