namespace Library.Application.DTOs;

public record AuthorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Biography,
    int BookCount);

public record CreateAuthorDto(
    string FirstName,
    string LastName,
    string? Biography);

public record UpdateAuthorDto(
    string FirstName,
    string LastName,
    string? Biography);
