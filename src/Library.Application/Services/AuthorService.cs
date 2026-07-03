using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Application.Mapping;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authors;

    public AuthorService(IAuthorRepository authors) => _authors = authors;

    public async Task<Result<IReadOnlyList<AuthorDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var authors = await _authors.GetAllAsync(ct);
        IReadOnlyList<AuthorDto> dtos = authors.Select(a => a.ToDto()).ToList();
        return Result<IReadOnlyList<AuthorDto>>.Success(dtos);
    }

    public async Task<Result<AuthorDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        return author is null
            ? Result<AuthorDto>.Failure($"Author with id '{id}' was not found.", ErrorType.NotFound)
            : Result<AuthorDto>.Success(author.ToDto());
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default)
    {
        var validation = Validate(dto.FirstName, dto.LastName);
        if (validation is not null)
            return Result<AuthorDto>.Failure(validation, ErrorType.Validation);

        var author = new Author
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Biography = dto.Biography?.Trim()
        };

        await _authors.AddAsync(author, ct);
        await _authors.SaveChangesAsync(ct);
        return Result<AuthorDto>.Success(author.ToDto());
    }

    public async Task<Result<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken ct = default)
    {
        var validation = Validate(dto.FirstName, dto.LastName);
        if (validation is not null)
            return Result<AuthorDto>.Failure(validation, ErrorType.Validation);

        var author = await _authors.GetByIdAsync(id, ct);
        if (author is null)
            return Result<AuthorDto>.Failure($"Author with id '{id}' was not found.", ErrorType.NotFound);

        author.FirstName = dto.FirstName.Trim();
        author.LastName = dto.LastName.Trim();
        author.Biography = dto.Biography?.Trim();

        _authors.Update(author);
        await _authors.SaveChangesAsync(ct);
        return Result<AuthorDto>.Success(author.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        if (author is null)
            return Result.Failure($"Author with id '{id}' was not found.", ErrorType.NotFound);

        if (author.Books.Count > 0)
            return Result.Failure("Cannot delete an author linked to one or more books.", ErrorType.Conflict);

        _authors.Remove(author);
        await _authors.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static string? Validate(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return "Author first name is required.";
        if (string.IsNullOrWhiteSpace(lastName))
            return "Author last name is required.";
        return null;
    }
}
