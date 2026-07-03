using Library.Application.DTOs;
using Library.Domain.Entities;

namespace Library.Application.Mapping;

public static class MappingExtensions
{
    public static BookDto ToDto(this Book book) => new(
        book.Id,
        book.Name,
        book.Description,
        book.Price,
        book.Discount,
        FinalPrice: Math.Max(0, book.Price - book.Discount),
        book.PublicationDate,
        book.ImageUrl,
        book.Authors
            .OrderBy(a => a.LastName)
            .Select(a => new BookAuthorDto(a.Id, a.FirstName, a.LastName))
            .ToList());

    public static AuthorDto ToDto(this Author author) => new(
        author.Id,
        author.FirstName,
        author.LastName,
        author.Biography,
        author.Books?.Count ?? 0);

    public static UserDto ToDto(this User user) => new(
        user.Id,
        user.Email,
        user.FullName,
        user.Role.ToString());
}
