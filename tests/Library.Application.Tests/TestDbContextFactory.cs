using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Tests;

/// <summary>Spins up an isolated in-memory <see cref="LibraryDbContext"/> per test.</summary>
public static class TestDbContextFactory
{
    public static LibraryDbContext Create()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase($"library-tests-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        return new LibraryDbContext(options);
    }
}
