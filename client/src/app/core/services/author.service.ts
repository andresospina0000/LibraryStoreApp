import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult, Author, CreateAuthor } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthorService {
  private readonly baseUrl = `${environment.apiUrl}/authors`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResult<Author[]>> {
    return this.http.get<ApiResult<Author[]>>(this.baseUrl);
  }

  create(author: CreateAuthor): Observable<ApiResult<Author>> {
    return this.http.post<ApiResult<Author>>(this.baseUrl, author);
  }

  update(id: string, author: CreateAuthor): Observable<ApiResult<Author>> {
    return this.http.put<ApiResult<Author>>(`${this.baseUrl}/${id}`, author);
  }

  delete(id: string): Observable<ApiResult<null>> {
    return this.http.delete<ApiResult<null>>(`${this.baseUrl}/${id}`);
  }
}
