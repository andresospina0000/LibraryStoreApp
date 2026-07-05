using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class BooksController(IBookService books) : ApiControllerBase
{
    private readonly IBookService _books = books;

    /// <summary>Public: paginated, alphabetically-ordered list of books.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
        => FromResult(await _books.GetAllAsync(page, pageSize, search, ct));

    /// <summary>Public: fetch a single book.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => FromResult(await _books.GetByIdAsync(id, ct));

    /// <summary>Admin only: create a book.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto, CancellationToken ct)
    {
        var result = await _books.CreateAsync(dto, ct);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : FromResult(result);
    }

    /// <summary>Admin only: update a book.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto, CancellationToken ct)
        => FromResult(await _books.UpdateAsync(id, dto, ct));

    /// <summary>Admin only: delete a book.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => FromResult(await _books.DeleteAsync(id, ct));
}
