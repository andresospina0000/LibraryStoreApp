import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult, AuthResult, User } from '../models/api.models';

const TOKEN_KEY = 'library.token';
const USER_KEY = 'library.user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/users`;

  private readonly _user = signal<User | null>(this.loadUser());
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._user() !== null);
  readonly isAdmin = computed(() => this._user()?.role === 'Admin');

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<ApiResult<AuthResult>> {
    return this.http
      .post<ApiResult<AuthResult>>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap((res) => {
          if (res.succeeded && res.data) {
            localStorage.setItem(TOKEN_KEY, res.data.token);
            localStorage.setItem(USER_KEY, JSON.stringify(res.data.user));
            this._user.set(res.data.user);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._user.set(null);
  }

  updatePassword(currentPassword: string, newPassword: string): Observable<ApiResult<null>> {
    return this.http.put<ApiResult<null>>(`${this.baseUrl}/me/password`, {
      currentPassword,
      newPassword
    });
  }

  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private loadUser(): User | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as User) : null;
  }
}
