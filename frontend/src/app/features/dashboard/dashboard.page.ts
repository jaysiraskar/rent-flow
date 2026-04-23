import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from './dashboard.service';
import { DashboardDueItem, DashboardSummary } from '../../shared/models/rent-record.models';

@Component({
  standalone: true,
  selector: 'app-dashboard-page',
  imports: [CommonModule, RouterLink],
  template: `
  <div class="container grid">
    <h2>Dashboard</h2>

    <div *ngIf="error" class="card error">{{error}}</div>

    <div class="grid grid-4" *ngIf="summary">
      <div class="card"><div class="muted">Total Tenants</div><h3>{{summary.totalTenants}}</h3></div>
      <div class="card"><div class="muted">Total Due</div><h3>₹{{summary.totalDue}}</h3></div>
      <div class="card"><div class="muted">Collected</div><h3>₹{{summary.collectedAmount}}</h3></div>
      <div class="card"><div class="muted">Pending</div><h3>₹{{summary.pendingAmount}}</h3></div>
    </div>

    <div class="grid" *ngIf="loading">
      <div class="card">Loading dashboard insights...</div>
    </div>

    <div class="grid grid-2" *ngIf="!loading">
      <div class="card">
        <div class="row justify-between">
          <h3>Upcoming Dues (7 days)</h3>
          <a routerLink="/rent-records">View all</a>
        </div>
        <table>
          <thead><tr><th>Tenant</th><th>Property</th><th>Due</th><th>Pending</th><th>Status</th></tr></thead>
          <tbody>
            <tr *ngFor="let item of upcomingDues">
              <td>{{item.tenantName}}</td>
              <td>{{item.propertyName}}</td>
              <td>{{item.dueDate}}</td>
              <td>₹{{item.pendingAmount}}</td>
              <td>{{item.status}}</td>
            </tr>
            <tr *ngIf="upcomingDues.length === 0"><td colspan="5" class="muted">No upcoming dues in next 7 days.</td></tr>
          </tbody>
        </table>
      </div>

      <div class="card">
        <div class="row justify-between">
          <h3>Overdue</h3>
          <a routerLink="/rent-records">View all</a>
        </div>
        <table>
          <thead><tr><th>Tenant</th><th>Property</th><th>Due</th><th>Pending</th><th>Status</th></tr></thead>
          <tbody>
            <tr *ngFor="let item of overdueDues">
              <td>{{item.tenantName}}</td>
              <td>{{item.propertyName}}</td>
              <td>{{item.dueDate}}</td>
              <td>₹{{item.pendingAmount}}</td>
              <td>{{item.status}}</td>
            </tr>
            <tr *ngIf="overdueDues.length === 0"><td colspan="5" class="muted">No overdue dues 🎉</td></tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>`
})
export class DashboardPage implements OnInit {
  summary?: DashboardSummary;
  upcomingDues: DashboardDueItem[] = [];
  overdueDues: DashboardDueItem[] = [];
  loading = false;
  error = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    const now = new Date();
    this.loading = true;

    this.dashboardService.getSummary(now.getFullYear(), now.getMonth() + 1).subscribe({
      next: (res) => this.summary = res,
      error: (e) => this.error = e?.error?.error ?? 'Failed to load dashboard summary'
    });

    this.dashboardService.getUpcomingDues(7).subscribe({
      next: (res) => this.upcomingDues = res,
      error: (e) => this.error = e?.error?.error ?? 'Failed to load upcoming dues'
    });

    this.dashboardService.getOverdue().subscribe({
      next: (res) => {
        this.overdueDues = res;
        this.loading = false;
      },
      error: (e) => {
        this.error = e?.error?.error ?? 'Failed to load overdue dues';
        this.loading = false;
      }
    });
  }
}
