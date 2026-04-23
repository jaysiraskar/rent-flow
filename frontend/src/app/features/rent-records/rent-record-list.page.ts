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

    <div class="card row">
      <input type="number" [(ngModel)]="year" placeholder="Year" style="max-width:120px" />
      <input type="number" [(ngModel)]="month" placeholder="Month" style="max-width:120px" />
      <select [(ngModel)]="propertyId"><option value="">All properties</option><option *ngFor="let p of properties" [value]="p.id">{{p.name}}</option></select>
      <select [(ngModel)]="status"><option value="">All status</option><option>Unpaid</option><option>Partial</option><option>Paid</option></select>
      <button (click)="load()">Filter</button>
      <button (click)="generate()">Generate Monthly</button>
    </div>

    <div class="card">
      <table>
        <thead><tr><th>Tenant</th><th>Property</th><th>Due</th><th>Expected</th><th>Paid</th><th>Status</th><th></th></tr></thead>
        <tbody>
          <tr *ngFor="let r of records">
            <td>{{r.tenantName}}</td><td>{{r.propertyName}}</td><td>{{r.dueDate}}</td><td>₹{{r.expectedAmount}}</td><td>₹{{r.paidAmount}}</td><td>{{r.status}}</td>
            <td><button (click)="markPaid(r)">Mark Paid</button></td>
          </tr>
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
  properties: Property[] = [];
  records: RentRecord[] = [];

  constructor(private recordsService: RentRecordsService, private propertiesService: PropertiesService) {}

  ngOnInit() {
    this.propertiesService.list().subscribe((res) => this.properties = res);
    this.load();
  }

  load() { this.recordsService.list(this.year, this.month, this.propertyId || undefined, this.status || undefined).subscribe((res) => this.records = res); }

  generate() { this.recordsService.generateMonthly(this.year, this.month, this.propertyId || undefined).subscribe(() => this.load()); }

  markPaid(record: RentRecord) { this.recordsService.updatePayment(record.id, record.expectedAmount, 'Collected').subscribe(() => this.load()); }
}
