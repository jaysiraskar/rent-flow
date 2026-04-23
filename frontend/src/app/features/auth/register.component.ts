import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
  <div class="container" style="max-width:520px; margin-top:3rem;">
    <div class="card">
      <h2>Create account</h2>
      <form [formGroup]="form" (ngSubmit)="submit()" class="grid">
        <input formControlName="fullName" placeholder="Full name" />
        <input formControlName="email" placeholder="Email" />
        <input formControlName="phoneNumber" placeholder="Phone" />
        <input type="password" formControlName="password" placeholder="Password" />
        <div *ngIf="error" class="error">{{ error }}</div>
        <button type="submit" [disabled]="form.invalid">Register</button>
      </form>
      <p class="muted" style="margin-top:.75rem;">Already have account? <a routerLink="/login">Login</a></p>
    </div>
  </div>`
})
export class RegisterComponent {
  error = '';
  form: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.form = this.fb.nonNullable.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.error = '';
    this.authService.register(this.form.getRawValue()).subscribe({
      next: (res) => this.authService.handleAuthSuccess(res),
      error: (e) => this.error = e?.error?.error ?? 'Registration failed'
    });
  }
}
