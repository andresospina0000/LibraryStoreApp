using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public class AuthorRepository(LibraryDbContext db) : IAuthorRepository
{
    private readonly LibraryDbContext _db = db;

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Authors
            .Include(a => a.Books) // eager loading (for book counts)
            .OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<List<Author>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idSet = ids.Distinct().ToList();
        return await _db.Authors
            .Where(a => idSet.Contains(a.Id))
            .ToListAsync(ct);
    }

    public async Task AddAsync(Author author, CancellationToken ct = default) =>
        await _db.Authors.AddAsync(author, ct);

    public void Update(Author author) => _db.Authors.Update(author);

    public void Remove(Author author) => _db.Authors.Remove(author);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}
