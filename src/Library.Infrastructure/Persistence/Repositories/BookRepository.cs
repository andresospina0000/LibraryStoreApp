using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public class BookRepository(LibraryDbContext db) : IBookRepository
{
    private readonly LibraryDbContext _db = db;

    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _db.Books
            .Include(b => b.Authors) // eager loading
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(b => b.Name.ToLower().Contains(term));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Name) // alphabetical order
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task AddAsync(Book book, CancellationToken ct = default) =>
        await _db.Books.AddAsync(book, ct);

    public void Update(Book book) => _db.Books.Update(book);

    public void Remove(Book book) => _db.Books.Remove(book);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}
