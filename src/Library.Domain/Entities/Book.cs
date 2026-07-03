namespace Library.Domain.Entities;

/// <summary>
/// A book available for selling in the library store.
/// </summary>
public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>List price of the book.</summary>
    public decimal Price { get; set; }

    /// <summary>Absolute discount amount subtracted from the price.</summary>
    public decimal Discount { get; set; }

    public DateTime? PublicationDate { get; set; }

    /// <summary>URL or data-uri of the book cover image.</summary>
    public string? ImageUrl { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation
    public ICollection<Author> Authors { get; set; } = new List<Author>();
}
