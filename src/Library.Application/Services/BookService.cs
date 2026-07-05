using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Application.Mapping;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class BookService(IBookRepository books, IAuthorRepository authors) : IBookService
{
    private readonly IBookRepository _books = books;
    private readonly IAuthorRepository _authors = authors;

    public async Task<Result<PagedResult<BookDto>>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var (items, total) = await _books.GetPagedAsync(page, pageSize, search, ct);

        var paged = new PagedResult<BookDto>
        {
            Items = items.Select(b => b.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
        return Result<PagedResult<BookDto>>.Success(paged);
    }

    public async Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        return book is null
            ? Result<BookDto>.Failure($"Book with id '{id}' was not found.", ErrorType.NotFound)
            : Result<BookDto>.Success(book.ToDto());
    }

    public async Task<Result<BookDto>> CreateAsync(CreateBookDto dto, CancellationToken ct = default)
    {
        var validation = Validate(dto.Name, dto.Description, dto.Price, dto.Discount, dto.AuthorIds);
        if (validation is not null)
            return Result<BookDto>.Failure(validation, ErrorType.Validation);

        var authors = await _authors.GetByIdsAsync(dto.AuthorIds, ct);
        if (authors.Count != dto.AuthorIds.Distinct().Count())
            return Result<BookDto>.Failure("One or more authors were not found.", ErrorType.NotFound);

        var book = new Book
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            Discount = dto.Discount,
            PublicationDate = dto.PublicationDate,
            ImageUrl = dto.ImageUrl,
            Authors = authors
        };

        await _books.AddAsync(book, ct);
        await _books.SaveChangesAsync(ct);

        return Result<BookDto>.Success(book.ToDto());
    }

    public async Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken ct = default)
    {
        var validation = Validate(dto.Name, dto.Description, dto.Price, dto.Discount, dto.AuthorIds);
        if (validation is not null)
            return Result<BookDto>.Failure(validation, ErrorType.Validation);

        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            return Result<BookDto>.Failure($"Book with id '{id}' was not found.", ErrorType.NotFound);

        var authors = await _authors.GetByIdsAsync(dto.AuthorIds, ct);
        if (authors.Count != dto.AuthorIds.Distinct().Count())
            return Result<BookDto>.Failure("One or more authors were not found.", ErrorType.NotFound);

        book.Name = dto.Name.Trim();
        book.Description = dto.Description.Trim();
        book.Price = dto.Price;
        book.Discount = dto.Discount;
        book.PublicationDate = dto.PublicationDate;
        book.ImageUrl = dto.ImageUrl;
        book.UpdatedAtUtc = DateTime.UtcNow;

        book.Authors.Clear();
        foreach (var a in authors) book.Authors.Add(a);

        _books.Update(book);
        await _books.SaveChangesAsync(ct);

        return Result<BookDto>.Success(book.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            return Result.Failure($"Book with id '{id}' was not found.", ErrorType.NotFound);

        _books.Remove(book);
        await _books.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static string? Validate(string name, string description, decimal price, decimal discount, IReadOnlyList<Guid> authorIds)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Book name is required.";
        if (string.IsNullOrWhiteSpace(description))
            return "Book description is required.";
        if (price <= 0)
            return "Price must be greater than zero.";
        if (discount < 0)
            return "Discount cannot be negative.";
        if (discount > price)
            return "Discount cannot exceed the price.";
        if (authorIds is null || authorIds.Count == 0)
            return "At least one author is required.";
        return null;
    }
}
