using System.Security.Cryptography;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Library.Infrastructure.Security;

/// <summary>PBKDF2 (HMAC-SHA256) password hasher. Format: {iterations}.{saltB64}.{hashB64}.</summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = KeyDerivation.Pbkdf2(password, salt, Prf, Iterations, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out var iterations)) return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = KeyDerivation.Pbkdf2(password, salt, Prf, iterations, expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
