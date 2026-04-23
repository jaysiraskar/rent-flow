import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from './dashboard.service';
import { DashboardSummary } from '../../shared/models/rent-record.models';

@Component({
  standalone: true,
  selector: 'app-dashboard-page',
  imports: [CommonModule],
  template: `
  <div class="container grid">
    <h2>Dashboard</h2>
    <div class="grid grid-4" *ngIf="summary">
      <div class="card"><div class="muted">Total Tenants</div><h3>{{summary.totalTenants}}</h3></div>
      <div class="card"><div class="muted">Total Due</div><h3>₹{{summary.totalDue}}</h3></div>
      <div class="card"><div class="muted">Collected</div><h3>₹{{summary.collectedAmount}}</h3></div>
      <div class="card"><div class="muted">Pending</div><h3>₹{{summary.pendingAmount}}</h3></div>
    </div>
  </div>`
})
export class DashboardPage implements OnInit {
  summary?: DashboardSummary;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    const now = new Date();
    this.dashboardService.getSummary(now.getFullYear(), now.getMonth() + 1).subscribe((res) => this.summary = res);
  }
}
