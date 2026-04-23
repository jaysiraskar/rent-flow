import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TenantsService } from '../tenants/tenants.service';
import { Tenant } from '../../shared/models/tenant.models';

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="container grid">
    <h2>Property Tenants</h2>
    <div class="card">
      <table>
        <thead><tr><th>Name</th><th>Room/Bed</th><th>Rent</th><th>Due day</th></tr></thead>
        <tbody>
          <tr *ngFor="let t of tenants">
            <td>{{t.fullName}}</td><td>{{t.roomOrBed}}</td><td>₹{{t.monthlyRent}}</td><td>{{t.rentDueDay}}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class PropertyDetailPage implements OnInit {
  tenants: Tenant[] = [];

  constructor(private route: ActivatedRoute, private tenantService: TenantsService) {}

  ngOnInit() {
    const propertyId = this.route.snapshot.paramMap.get('id');
    if (propertyId) this.tenantService.listByProperty(propertyId).subscribe((res) => this.tenants = res);
  }
}
