using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class UsersController : ApiControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    /// <summary>Public: authenticate and receive a JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        => FromResult(await _users.LoginAsync(dto, ct));

    /// <summary>Admin only: create a new user (admin or customer).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
        => FromResult(await _users.CreateAsync(dto, ct));

    /// <summary>Authenticated: update the current user's password (the "myself" page).</summary>
    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> UpdateMyPassword([FromBody] UpdatePasswordDto dto, CancellationToken ct)
    {
        if (CurrentUserId == Guid.Empty)
            return FromResult(Result.Failure("Invalid token.", ErrorType.Unauthorized));

        return FromResult(await _users.UpdatePasswordAsync(CurrentUserId, dto, ct));
    }
}
