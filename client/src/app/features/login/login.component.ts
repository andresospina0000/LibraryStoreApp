import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  email = 'admin@library.com';
  password = 'Admin123!';
  readonly submitting = signal(false);

  submit(): void {
    if (!this.email || !this.password) {
      this.toast.error('Email and password are required.');
      return;
    }
    this.submitting.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        this.submitting.set(false);
        if (res.succeeded && res.data) {
          this.toast.success(`Welcome back, ${res.data.user.fullName}!`);
          this.router.navigate([res.data.user.role === 'Admin' ? '/manage/books' : '/store']);
        } else {
          this.toast.error(res.error ?? 'Login failed.');
        }
      },
      error: (err) => {
        this.submitting.set(false);
        this.toast.error(err?.error?.error ?? 'Invalid email or password.');
      }
    });
  }
}
