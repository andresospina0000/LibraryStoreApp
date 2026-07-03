namespace Library.Application.DTOs;

public record BookAuthorDto(Guid Id, string FirstName, string LastName);

public record BookDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    decimal FinalPrice,
    DateTime? PublicationDate,
    string? ImageUrl,
    IReadOnlyList<BookAuthorDto> Authors);

public record CreateBookDto(
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    DateTime? PublicationDate,
    string? ImageUrl,
    IReadOnlyList<Guid> AuthorIds);

public record UpdateBookDto(
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    DateTime? PublicationDate,
    string? ImageUrl,
    IReadOnlyList<Guid> AuthorIds);
