namespace Library.Domain.Entities;

/// <summary>
/// An author that can be linked to N books (many-to-many).
/// </summary>
public class Author
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Biography { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Book> Books { get; set; } = [];
}
