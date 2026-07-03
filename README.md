# Library Management App

A full-stack library store: **.NET 8 Web API** (clean architecture) + **Angular 18 SPA** (Bootstrap light theme).

Non-registered users can browse books for sale. Admins can sign in to manage books, authors and their own password. A book can be linked to N authors.

## Solution structure

```
AndresOspinaNetApp/
├── Library.sln
├── library-db.sql                 # SQLite schema + mocked seed data (admin included)
├── src/
│   ├── Library.Domain/            # Entities (Book, Author, User), enums — no dependencies
│   ├── Library.Application/       # Use-case services, DTOs, interfaces, Result<T> envelope, errors
│   ├── Library.Infrastructure/    # EF Core (code-first), DbContext, configs, repositories, JWT, hashing
│   └── Library.API/               # Controllers, DTOs, global exception middleware, DI, auth
├── tests/
│   └── Library.Application.Tests/ # TDD unit tests for the CRUD business logic (21 tests)
└── client/                        # Angular 18 SPA
```

**Dependency direction:** API → Infrastructure → Application → Domain. Domain has no dependencies.

## Backend highlights

- **Clean architecture** with interface/implementation split; all services & repositories registered **scoped** in `Program.cs` via `AddApplication()` / `AddInfrastructure()`.
- **EF Core code-first** on **SQLite** (`library-db.sqlite`), created & seeded on startup. Many-to-many `Book`↔`Author` via a `BookAuthors` join table.
- **GUID primary keys** on every entity.
- **Eager loading** (`Include`) everywhere; **paginated + alphabetically ordered** `GET /api/books`.
- **Standardized response** `Result<T>` — `succeeded`, `data`, `error`, `errorType`.
- **Global exception middleware** maps error types → HTTP codes (400/401/403/404/409/422/500).
- **JWT auth**; only `Admin` role can create/update/delete books & authors. PBKDF2 password hashing.

### API endpoints

| Verb   | Route                       | Access        |
|--------|-----------------------------|---------------|
| GET    | `/api/books`                | Public (paged)|
| GET    | `/api/books/{id}`           | Public        |
| POST   | `/api/books`                | Admin         |
| PUT    | `/api/books/{id}`           | Admin         |
| DELETE | `/api/books/{id}`           | Admin         |
| GET    | `/api/authors`              | Public        |
| POST/PUT/DELETE | `/api/authors[/{id}]` | Admin      |
| POST   | `/api/users/login`          | Public        |
| POST   | `/api/users`                | Admin         |
| PUT    | `/api/users/me/password`    | Authenticated |

### Seeded accounts

| Email                  | Password       | Role     |
|------------------------|----------------|----------|
| admin@library.com      | `Admin123!`    | Admin    |
| customer@library.com   | `Customer123!` | Customer |

## Frontend highlights

- Angular 18 standalone components, lazy-loaded routes, **signals**, **RxJS** HTTP services.
- Pages: `/store` (public), `/login`, `/manage/books`, `/manage/authors`, `/manage/myself`.
- **HTTP interceptor** attaches the JWT and handles 401/403 globally.
- **Route guard** (`adminGuard`) redirects unauthorized users to `/store`.
- **Toasts** for create/update/delete/error feedback.
- **Navbar** shows management modules only to admins.

## Running

### 1. API (http://localhost:5264, Swagger at `/swagger`)

```bash
cd src/Library.API
dotnet run
```

### 2. Angular SPA (http://localhost:4200)

```bash
cd client
npm install     # first time only
npm start       # ng serve
```

Open http://localhost:4200, browse the store, then sign in as the admin above to manage content.

### Tests

```bash
dotnet test
```

## Notes

- `library-db.sql` is a portable dump of the seeded schema + data. The API also auto-creates/seeds
  `library-db.sqlite` on first run, so the SQL file is provided for reference/manual provisioning.
- The JWT signing key in `appsettings.json` is a development placeholder — replace it for production.
