namespace Library.Application.DTOs;

public record UserDto(Guid Id, string Email, string FullName, string Role);

public record LoginDto(string Email, string Password);

public record CreateUserDto(string Email, string FullName, string Password, string Role);

public record UpdatePasswordDto(string CurrentPassword, string NewPassword);

public record AuthResultDto(string Token, DateTime ExpiresAtUtc, UserDto User);
