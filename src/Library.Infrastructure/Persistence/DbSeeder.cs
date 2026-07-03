using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence;

/// <summary>
/// Applies migrations/ensures the schema and seeds an admin user plus demo data.
/// Uses deterministic Guids so the generated data matches library-db.sql.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(LibraryDbContext db, IPasswordHasher hasher)
    {
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Email = "admin@library.com",
                    FullName = "Library Administrator",
                    Role = UserRole.Admin,
                    PasswordHash = hasher.Hash("Admin123!")
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Email = "customer@library.com",
                    FullName = "Demo Customer",
                    Role = UserRole.Customer,
                    PasswordHash = hasher.Hash("Customer123!")
                });
        }

        if (!await db.Authors.AnyAsync())
        {
            var orwell = new Author { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), FirstName = "George", LastName = "Orwell", Biography = "English novelist and essayist." };
            var austen = new Author { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), FirstName = "Jane", LastName = "Austen", Biography = "English novelist known for romantic fiction." };
            var tolkien = new Author { Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"), FirstName = "J.R.R.", LastName = "Tolkien", Biography = "Author of high-fantasy classics." };
            var pratchett = new Author { Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"), FirstName = "Terry", LastName = "Pratchett", Biography = "Author of the Discworld series." };
            var gaiman = new Author { Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"), FirstName = "Neil", LastName = "Gaiman", Biography = "Author of fantasy and graphic novels." };

            db.Authors.AddRange(orwell, austen, tolkien, pratchett, gaiman);

            db.Books.AddRange(
                new Book
                {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                    Name = "1984",
                    Description = "A dystopian social science fiction novel about totalitarian control.",
                    Price = 19.99m, Discount = 2.00m,
                    PublicationDate = new DateTime(1949, 6, 8),
                    ImageUrl = "https://covers.openlibrary.org/b/id/7222246-M.jpg",
                    Authors = new List<Author> { orwell }
                },
                new Book
                {
                    Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                    Name = "Animal Farm",
                    Description = "A satirical allegorical novella about a farm animal rebellion.",
                    Price = 14.99m, Discount = 0m,
                    PublicationDate = new DateTime(1945, 8, 17),
                    ImageUrl = "https://covers.openlibrary.org/b/id/8575741-M.jpg",
                    Authors = new List<Author> { orwell }
                },
                new Book
                {
                    Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
                    Name = "Pride and Prejudice",
                    Description = "A romantic novel of manners set in Georgian England.",
                    Price = 12.50m, Discount = 1.50m,
                    PublicationDate = new DateTime(1813, 1, 28),
                    ImageUrl = "https://covers.openlibrary.org/b/id/8091016-M.jpg",
                    Authors = new List<Author> { austen }
                },
                new Book
                {
                    Id = Guid.Parse("b4444444-4444-4444-4444-444444444444"),
                    Name = "The Hobbit",
                    Description = "A fantasy adventure following Bilbo Baggins on a quest.",
                    Price = 24.99m, Discount = 5.00m,
                    PublicationDate = new DateTime(1937, 9, 21),
                    ImageUrl = "https://covers.openlibrary.org/b/id/6979861-M.jpg",
                    Authors = new List<Author> { tolkien }
                },
                new Book
                {
                    Id = Guid.Parse("b5555555-5555-5555-5555-555555555555"),
                    Name = "Good Omens",
                    Description = "A comedy about the birth of the Antichrist and the coming apocalypse.",
                    Price = 22.00m, Discount = 3.00m,
                    PublicationDate = new DateTime(1990, 5, 1),
                    ImageUrl = "https://covers.openlibrary.org/b/id/8778971-M.jpg",
                    Authors = new List<Author> { pratchett, gaiman }
                });
        }

        await db.SaveChangesAsync();
    }
}
