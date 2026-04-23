import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../../shared/models/auth.models';
import { StorageService } from './storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly isLoggedIn = signal<boolean>(false);

  constructor(private http: HttpClient, private storage: StorageService, private router: Router) {
    this.isLoggedIn.set(!!this.storage.token);
  }

  login(payload: LoginRequest) {
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/login`, payload);
  }

  register(payload: RegisterRequest) {
    return this.http.post<AuthResponse>(`${environment.apiBaseUrl}/auth/register`, payload);
  }

  handleAuthSuccess(response: AuthResponse) {
    this.storage.token = response.token;
    this.isLoggedIn.set(true);
    this.router.navigateByUrl('/dashboard');
  }

  logout() {
    this.storage.token = null;
    this.isLoggedIn.set(false);
    this.router.navigateByUrl('/login');
  }
}
