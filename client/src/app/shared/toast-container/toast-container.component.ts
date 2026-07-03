import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 1090">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast show align-items-center text-white border-0 mb-2"
          [class.bg-success]="toast.type === 'success'"
          [class.bg-danger]="toast.type === 'danger'"
          [class.bg-primary]="toast.type === 'info'"
          [class.bg-warning]="toast.type === 'warning'"
          role="alert"
        >
          <div class="d-flex">
            <div class="toast-body">{{ toast.message }}</div>
            <button
              type="button"
              class="btn-close btn-close-white me-2 m-auto"
              (click)="toastService.dismiss(toast.id)"
              aria-label="Close"
            ></button>
          </div>
        </div>
      }
    </div>
  `
})
export class ToastContainerComponent {
  readonly toastService = inject(ToastService);
}
