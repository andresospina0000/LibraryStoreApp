using Library.Domain.Entities;

namespace Library.Application.Interfaces;

public interface IBookRepository
{
    /// <summary>Returns an ordered, paginated slice of books with authors eager-loaded.</summary>
    Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken ct = default);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(Book book, CancellationToken ct = default);

    void Update(Book book);

    void Remove(Book book);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct = default);

    Task<Author?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Loads all authors whose ids are in the supplied set (eager for book linking).</summary>
    Task<List<Author>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

    Task AddAsync(Author author, CancellationToken ct = default);

    void Update(Author author);

    void Remove(Author author);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

    Task AddAsync(User user, CancellationToken ct = default);

    void Update(User user);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
