import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

/**
 * Attaches the JWT bearer token to outgoing requests and centralizes
 * auth-error handling (401 -> logout & redirect, 403 -> store).
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);

  const token = auth.token;
  const request = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(request).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401) {
        auth.logout();
        toast.error('Your session expired. Please sign in again.');
        router.navigate(['/login']);
      } else if (err.status === 403) {
        toast.error('You are not authorized to perform this action.');
        router.navigate(['/store']);
      }
      return throwError(() => err);
    })
  );
};
