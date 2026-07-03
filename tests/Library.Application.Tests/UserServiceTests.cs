using FluentAssertions;
using Library.Application.Common;
using Library.Application.DTOs;
using Library.Application.Services;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repositories;
using Library.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace Library.Application.Tests;

public class UserServiceTests
{
    private static UserService CreateSut(LibraryDbContext db)
    {
        var jwt = new JwtTokenGenerator(Options.Create(new JwtSettings
        {
            Key = "super-secret-signing-key-for-unit-tests-1234567890",
            Issuer = "test", Audience = "test", ExpiryMinutes = 60
        }));
        return new UserService(new UserRepository(db), new PasswordHasher(), jwt);
    }

    [Fact]
    public async Task CreateAsync_ThenLogin_ReturnsToken()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);

        var created = await service.CreateAsync(new CreateUserDto("admin@x.com", "Admin", "secret123", "Admin"));
        created.Succeeded.Should().BeTrue();

        var login = await service.LoginAsync(new LoginDto("admin@x.com", "secret123"));
        login.Succeeded.Should().BeTrue();
        login.Data!.Token.Should().NotBeNullOrWhiteSpace();
        login.Data.User.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);
        await service.CreateAsync(new CreateUserDto("a@x.com", "A", "secret123", "Customer"));

        var login = await service.LoginAsync(new LoginDto("a@x.com", "wrongpass"));

        login.Succeeded.Should().BeFalse();
        login.ErrorType.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ReturnsConflict()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);
        await service.CreateAsync(new CreateUserDto("dup@x.com", "A", "secret123", "Customer"));

        var second = await service.CreateAsync(new CreateUserDto("dup@x.com", "B", "secret123", "Customer"));

        second.ErrorType.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_ShortPassword_FailsValidation()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);
        var result = await service.CreateAsync(new CreateUserDto("a@x.com", "A", "123", "Customer"));
        result.ErrorType.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdatePassword_WithCorrectCurrent_Succeeds()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);
        var created = await service.CreateAsync(new CreateUserDto("a@x.com", "A", "secret123", "Admin"));

        var update = await service.UpdatePasswordAsync(created.Data!.Id,
            new UpdatePasswordDto("secret123", "newsecret123"));
        update.Succeeded.Should().BeTrue();

        var login = await service.LoginAsync(new LoginDto("a@x.com", "newsecret123"));
        login.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePassword_WithWrongCurrent_FailsUnauthorized()
    {
        var db = TestDbContextFactory.Create();
        var service = CreateSut(db);
        var created = await service.CreateAsync(new CreateUserDto("a@x.com", "A", "secret123", "Admin"));

        var update = await service.UpdatePasswordAsync(created.Data!.Id,
            new UpdatePasswordDto("wrongcurrent", "newsecret123"));

        update.ErrorType.Should().Be(ErrorType.Unauthorized);
    }
}
