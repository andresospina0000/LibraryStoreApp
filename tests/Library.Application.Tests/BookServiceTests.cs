using FluentAssertions;
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repositories;

namespace Library.Application.Tests;

public class BookServiceTests
{
    private static (BookService service, LibraryDbContext db) CreateSut()
    {
        var db = TestDbContextFactory.Create();
        var service = new BookService(new BookRepository(db), new AuthorRepository(db));
        return (service, db);
    }

    private static Author SeedAuthor(LibraryDbContext db, string first = "Test", string last = "Author")
    {
        var author = new Author { FirstName = first, LastName = last };
        db.Authors.Add(author);
        db.SaveChanges();
        return author;
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesBookAndLinksAuthors()
    {
        var (service, db) = CreateSut();
        var author = SeedAuthor(db);

        var dto = new CreateBookDto("Clean Code", "A handbook of craftsmanship", 40m, 5m,
            new DateTime(2008, 8, 1), null, new[] { author.Id });

        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Data!.Name.Should().Be("Clean Code");
        result.Data.FinalPrice.Should().Be(35m);
        result.Data.Authors.Should().ContainSingle(a => a.Id == author.Id);
    }

    [Fact]
    public async Task CreateAsync_WithNoAuthors_FailsValidation()
    {
        var (service, _) = CreateSut();

        var dto = new CreateBookDto("Book", "Desc", 10m, 0m, null, null, Array.Empty<Guid>());
        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_WithDiscountGreaterThanPrice_FailsValidation()
    {
        var (service, db) = CreateSut();
        var author = SeedAuthor(db);

        var dto = new CreateBookDto("Book", "Desc", 10m, 20m, null, null, new[] { author.Id });
        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownAuthor_FailsNotFound()
    {
        var (service, _) = CreateSut();

        var dto = new CreateBookDto("Book", "Desc", 10m, 0m, null, null, new[] { Guid.NewGuid() });
        var result = await service.CreateAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNotFound()
    {
        var (service, _) = CreateSut();
        var result = await service.GetByIdAsync(Guid.NewGuid());
        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ChangesFieldsAndAuthors()
    {
        var (service, db) = CreateSut();
        var a1 = SeedAuthor(db, "Anna", "Alpha");
        var a2 = SeedAuthor(db, "Bob", "Beta");

        var created = await service.CreateAsync(
            new CreateBookDto("Old", "Old desc", 10m, 0m, null, null, new[] { a1.Id }));

        var updated = await service.UpdateAsync(created.Data!.Id,
            new UpdateBookDto("New", "New desc", 30m, 3m, null, "img.png", new[] { a2.Id }));

        updated.Succeeded.Should().BeTrue();
        updated.Data!.Name.Should().Be("New");
        updated.Data.FinalPrice.Should().Be(27m);
        updated.Data.Authors.Should().ContainSingle(a => a.Id == a2.Id);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBook()
    {
        var (service, db) = CreateSut();
        var author = SeedAuthor(db);
        var created = await service.CreateAsync(
            new CreateBookDto("Del", "Desc", 10m, 0m, null, null, new[] { author.Id }));

        var deleted = await service.DeleteAsync(created.Data!.Id);
        deleted.Succeeded.Should().BeTrue();

        var fetch = await service.GetByIdAsync(created.Data.Id);
        fetch.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllAsync_IsPaginatedAndOrderedAlphabetically()
    {
        var (service, db) = CreateSut();
        var author = SeedAuthor(db);
        foreach (var name in new[] { "Charlie", "Alpha", "Bravo", "Delta", "Echo" })
            await service.CreateAsync(new CreateBookDto(name, "Desc", 10m, 0m, null, null, new[] { author.Id }));

        var page1 = await service.GetAllAsync(page: 1, pageSize: 2, search: null);

        page1.Succeeded.Should().BeTrue();
        page1.Data!.TotalCount.Should().Be(5);
        page1.Data.TotalPages.Should().Be(3);
        page1.Data.Items.Select(b => b.Name).Should().ContainInOrder("Alpha", "Bravo");
    }

    [Fact]
    public async Task GetAllAsync_FiltersBySearch()
    {
        var (service, db) = CreateSut();
        var author = SeedAuthor(db);
        await service.CreateAsync(new CreateBookDto("The Great Gatsby", "Desc", 10m, 0m, null, null, new[] { author.Id }));
        await service.CreateAsync(new CreateBookDto("War and Peace", "Desc", 10m, 0m, null, null, new[] { author.Id }));

        var result = await service.GetAllAsync(1, 10, "gatsby");

        result.Data!.TotalCount.Should().Be(1);
        result.Data.Items.Single().Name.Should().Be("The Great Gatsby");
    }
}
