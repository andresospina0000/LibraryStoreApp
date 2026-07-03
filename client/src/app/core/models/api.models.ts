// Mirrors the API's standardized Result<T> envelope.
export interface ApiResult<T> {
  data: T | null;
  succeeded: boolean;
  error: string | null;
  errorType: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface BookAuthor {
  id: string;
  firstName: string;
  lastName: string;
}

export interface Book {
  id: string;
  name: string;
  description: string;
  price: number;
  discount: number;
  finalPrice: number;
  publicationDate: string | null;
  imageUrl: string | null;
  authors: BookAuthor[];
}

export interface CreateBook {
  name: string;
  description: string;
  price: number;
  discount: number;
  publicationDate: string | null;
  imageUrl: string | null;
  authorIds: string[];
}

export interface Author {
  id: string;
  firstName: string;
  lastName: string;
  biography: string | null;
  bookCount: number;
}

export interface CreateAuthor {
  firstName: string;
  lastName: string;
  biography: string | null;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export interface AuthResult {
  token: string;
  expiresAtUtc: string;
  user: User;
}
