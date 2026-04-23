import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { DashboardPage } from './features/dashboard/dashboard.page';
import { PropertyListPage } from './features/properties/property-list.page';
import { PropertyDetailPage } from './features/properties/property-detail.page';
import { TenantListPage } from './features/tenants/tenant-list.page';
import { RentRecordListPage } from './features/rent-records/rent-record-list.page';

export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', canActivate: [authGuard], component: DashboardPage },
  { path: 'properties', canActivate: [authGuard], component: PropertyListPage },
  { path: 'properties/:id', canActivate: [authGuard], component: PropertyDetailPage },
  { path: 'tenants', canActivate: [authGuard], component: TenantListPage },
  { path: 'rent-records', canActivate: [authGuard], component: RentRecordListPage },
  { path: '**', redirectTo: 'dashboard' }
];
