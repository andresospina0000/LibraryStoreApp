import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-myself',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './myself.component.html'
})
export class MyselfComponent {
  readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  readonly saving = signal(false);

  submit(): void {
    if (!this.currentPassword || !this.newPassword) {
      this.toast.error('All password fields are required.');
      return;
    }
    if (this.newPassword.length < 6) {
      this.toast.error('New password must be at least 6 characters.');
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.toast.error('New password and confirmation do not match.');
      return;
    }

    this.saving.set(true);
    this.auth.updatePassword(this.currentPassword, this.newPassword).subscribe({
      next: (res) => {
        this.saving.set(false);
        if (res.succeeded) {
          this.toast.success('Password updated successfully.');
          this.currentPassword = this.newPassword = this.confirmPassword = '';
        } else {
          this.toast.error(res.error ?? 'Could not update password.');
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.toast.error(err?.error?.error ?? 'Could not update password.');
      }
    });
  }
}
