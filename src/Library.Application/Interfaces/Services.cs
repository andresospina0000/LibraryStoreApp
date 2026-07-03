using Library.Application.Common;
using Library.Application.DTOs;

namespace Library.Application.Interfaces;

public interface IBookService
{
    Task<Result<PagedResult<BookDto>>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<BookDto>> CreateAsync(CreateBookDto dto, CancellationToken ct = default);
    Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IAuthorService
{
    Task<Result<IReadOnlyList<AuthorDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<AuthorDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default);
    Task<Result<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IUserService
{
    Task<Result<AuthResultDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<Result<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto, CancellationToken ct = default);
}
