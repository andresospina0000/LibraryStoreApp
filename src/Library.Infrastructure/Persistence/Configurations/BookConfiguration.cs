using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(300);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(4000);
        builder.Property(b => b.Price).HasColumnType("decimal(18,2)");
        builder.Property(b => b.Discount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.ImageUrl).HasMaxLength(2048);

        builder.HasIndex(b => b.Name);

        // Many-to-many Book <-> Author with an explicit join table.
        builder.HasMany(b => b.Authors)
            .WithMany(a => a.Books)
            .UsingEntity(j => j.ToTable("BookAuthors"));
    }
}
