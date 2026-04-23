import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: `
  <header *ngIf="showHeader()" style="background:#111827;color:white;padding:.75rem 1rem;">
    <div class="container row justify-between">
      <strong>RentFlow</strong>
      <nav class="row">
        <a routerLink="/dashboard" style="color:white;text-decoration:none;">Dashboard</a>
        <a routerLink="/properties" style="color:white;text-decoration:none;">Properties</a>
        <a routerLink="/tenants" style="color:white;text-decoration:none;">Tenants</a>
        <a routerLink="/rent-records" style="color:white;text-decoration:none;">Rent Records</a>
        <button (click)="logout()">Logout</button>
      </nav>
    </div>
  </header>
  <router-outlet></router-outlet>
  `
})
export class AppComponent {
  showHeader = false;

  constructor(private authService: AuthService, private router: Router) {
    this.updateHeaderVisibility(this.router.url);
    this.router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe((event) => {
      this.updateHeaderVisibility((event as NavigationEnd).urlAfterRedirects);
    });
  }

  private updateHeaderVisibility(url: string) {
    const isAuthPage = ['/login', '/register'].includes(url);
    this.showHeader = !isAuthPage && !!localStorage.getItem('rentflow_token');
  }

  logout() { this.authService.logout(); }
}
