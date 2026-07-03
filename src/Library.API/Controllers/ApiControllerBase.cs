using System.Security.Claims;
using Library.API.Middleware;
using Library.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Translates a payload-carrying <see cref="Result{T}"/> into an HTTP response.</summary>
    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.Succeeded)
            return Ok(result);

        var status = (int)ErrorTypeMapper.ToStatusCode(result.ErrorType);
        return StatusCode(status, result);
    }

    /// <summary>Translates a non-payload <see cref="Result"/> into an HTTP response.</summary>
    protected IActionResult FromResult(Result result)
    {
        if (result.Succeeded)
            return Ok(result);

        var status = (int)ErrorTypeMapper.ToStatusCode(result.ErrorType);
        return StatusCode(status, result);
    }

    /// <summary>Id of the authenticated user, taken from the JWT.</summary>
    protected Guid CurrentUserId
    {
        get
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
        }
    }
}
