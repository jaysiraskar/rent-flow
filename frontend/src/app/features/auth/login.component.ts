import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
  <div class="container" style="max-width:420px; margin-top:4rem;">
    <div class="card">
      <h2>RentFlow Login</h2>
      <p class="muted">Sign in to manage rents.</p>
      <form [formGroup]="form" (ngSubmit)="submit()" class="grid">
        <input formControlName="email" placeholder="Email" />
        <input type="password" formControlName="password" placeholder="Password" />
        <div *ngIf="error" class="error">{{ error }}</div>
        <button type="submit" [disabled]="form.invalid">Login</button>
      </form>
      <p class="muted" style="margin-top:.75rem;">No account? <a routerLink="/register">Register</a></p>
    </div>
  </div>`
})
export class LoginComponent {
  error = '';
  form: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.form = this.fb.nonNullable.group({ email: ['', [Validators.required, Validators.email]], password: ['', Validators.required] });
  }

  submit() {
    if (this.form.invalid) return;
    this.error = '';
    this.authService.login(this.form.getRawValue()).subscribe({
      next: (res) => this.authService.handleAuthSuccess(res),
      error: (e) => this.error = e?.error?.error ?? 'Login failed'
    });
  }
}
