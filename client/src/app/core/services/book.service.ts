import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult, Book, CreateBook, PagedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly baseUrl = `${environment.apiUrl}/books`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 8, search?: string): Observable<ApiResult<PagedResult<Book>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<ApiResult<PagedResult<Book>>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ApiResult<Book>> {
    return this.http.get<ApiResult<Book>>(`${this.baseUrl}/${id}`);
  }

  create(book: CreateBook): Observable<ApiResult<Book>> {
    return this.http.post<ApiResult<Book>>(this.baseUrl, book);
  }

  update(id: string, book: CreateBook): Observable<ApiResult<Book>> {
    return this.http.put<ApiResult<Book>>(`${this.baseUrl}/${id}`, book);
  }

  delete(id: string): Observable<ApiResult<null>> {
    return this.http.delete<ApiResult<null>>(`${this.baseUrl}/${id}`);
  }
}
