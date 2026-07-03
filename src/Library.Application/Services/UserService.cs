using System.Text.RegularExpressions;
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Application.Mapping;
using Library.Domain.Entities;
using Library.Domain.Enums;

namespace Library.Application.Services;

public class UserService : IUserService
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public UserService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthResultDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return Result<AuthResultDto>.Failure("Email and password are required.", ErrorType.Validation);

        var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return Result<AuthResultDto>.Failure("Invalid email or password.", ErrorType.Unauthorized);

        var (token, expires) = _jwt.Generate(user);
        return Result<AuthResultDto>.Success(new AuthResultDto(token, expires, user.ToDto()));
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || !EmailRegex.IsMatch(dto.Email))
            return Result<UserDto>.Failure("A valid email is required.", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.FullName))
            return Result<UserDto>.Failure("Full name is required.", ErrorType.Validation);
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return Result<UserDto>.Failure("Password must be at least 6 characters.", ErrorType.Validation);
        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
            return Result<UserDto>.Failure($"Invalid role '{dto.Role}'.", ErrorType.Validation);

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _users.EmailExistsAsync(email, ct))
            return Result<UserDto>.Failure($"A user with email '{email}' already exists.", ErrorType.Conflict);

        var user = new User
        {
            Email = email,
            FullName = dto.FullName.Trim(),
            PasswordHash = _hasher.Hash(dto.Password),
            Role = role
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return Result<UserDto>.Success(user.ToDto());
    }

    public async Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return Result.Failure("New password must be at least 6 characters.", ErrorType.Validation);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure("User was not found.", ErrorType.NotFound);

        if (!_hasher.Verify(dto.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect.", ErrorType.Unauthorized);

        user.PasswordHash = _hasher.Hash(dto.NewPassword);
        _users.Update(user);
        await _users.SaveChangesAsync(ct);
        return Result.Success();
    }
}
