import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PropertiesService } from './properties.service';
import { Property } from '../../shared/models/property.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
  <div class="container grid">
    <div class="row justify-between">
      <h2>Properties</h2>
      <button (click)="showForm = !showForm">{{showForm ? 'Cancel' : 'Add Property'}}</button>
    </div>

    <div class="card" *ngIf="showForm">
      <div class="grid">
        <input [(ngModel)]="form.name" placeholder="Property name" />
        <input [(ngModel)]="form.addressLine1" placeholder="Address" />
        <div class="row">
          <input [(ngModel)]="form.city" placeholder="City" />
          <input [(ngModel)]="form.state" placeholder="State" />
          <input [(ngModel)]="form.pincode" placeholder="Pincode" />
        </div>
        <button (click)="save()">Save</button>
      </div>
    </div>

    <div class="card">
      <table>
        <thead><tr><th>Name</th><th>Location</th><th></th></tr></thead>
        <tbody>
          <tr *ngFor="let p of properties">
            <td><a [routerLink]="['/properties', p.id]">{{p.name}}</a></td>
            <td>{{p.city}}, {{p.state}}</td>
            <td><button (click)="remove(p.id)">Delete</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>`
})
export class PropertyListPage implements OnInit {
  properties: Property[] = [];
  showForm = false;
  form = { name: '', addressLine1: '', city: '', state: '', pincode: '' };

  constructor(private propertiesService: PropertiesService) {}

  ngOnInit() { this.load(); }
  load() { this.propertiesService.list().subscribe((res) => this.properties = res); }
  save() { this.propertiesService.create(this.form).subscribe(() => { this.showForm = false; this.form = { name: '', addressLine1: '', city: '', state: '', pincode: '' }; this.load(); }); }
  remove(id: string) { this.propertiesService.remove(id).subscribe(() => this.load()); }
}
