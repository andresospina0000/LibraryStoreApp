using FluentAssertions;
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repositories;

namespace Library.Application.Tests;

public class AuthorServiceTests
{
    private static (AuthorService service, LibraryDbContext db) CreateSut()
    {
        var db = TestDbContextFactory.Create();
        return (new AuthorService(new AuthorRepository(db)), db);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_Succeeds()
    {
        var (service, _) = CreateSut();
        var result = await service.CreateAsync(new CreateAuthorDto("Isaac", "Asimov", "Sci-fi author"));

        result.Succeeded.Should().BeTrue();
        result.Data!.FirstName.Should().Be("Isaac");
        result.Data.LastName.Should().Be("Asimov");
    }

    [Fact]
    public async Task CreateAsync_WithBlankName_FailsValidation()
    {
        var (service, _) = CreateSut();
        var result = await service.CreateAsync(new CreateAuthorDto("", "Asimov", null));
        result.ErrorType.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_ChangesFields()
    {
        var (service, _) = CreateSut();
        var created = await service.CreateAsync(new CreateAuthorDto("Old", "Name", null));

        var updated = await service.UpdateAsync(created.Data!.Id, new UpdateAuthorDto("New", "Name", "bio"));

        updated.Succeeded.Should().BeTrue();
        updated.Data!.FirstName.Should().Be("New");
        updated.Data.Biography.Should().Be("bio");
    }

    [Fact]
    public async Task DeleteAsync_WhenLinkedToBook_FailsConflict()
    {
        var (service, db) = CreateSut();
        var author = new Author { FirstName = "Linked", LastName = "Author" };
        db.Authors.Add(author);
        db.Books.Add(new Book { Name = "B", Description = "D", Price = 5m, Authors = new List<Author> { author } });
        db.SaveChanges();

        var result = await service.DeleteAsync(author.Id);

        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteAsync_WhenUnlinked_Succeeds()
    {
        var (service, _) = CreateSut();
        var created = await service.CreateAsync(new CreateAuthorDto("Free", "Agent", null));

        var result = await service.DeleteAsync(created.Data!.Id);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ReturnsNotFound()
    {
        var (service, _) = CreateSut();
        var result = await service.DeleteAsync(Guid.NewGuid());
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }
}
