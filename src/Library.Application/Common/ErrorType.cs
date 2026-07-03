namespace Library.Application.Common;

/// <summary>
/// Application-level error categories. Mapped to HTTP status codes in the API layer.
/// </summary>
public enum ErrorType
{
    None = 0,
    Validation = 1,   // 400
    Unauthorized = 2, // 401
    Forbidden = 3,    // 403
    NotFound = 4,     // 404
    Conflict = 5,     // 409
    NotCreated = 6,   // 422
    ServerError = 7   // 500
}
