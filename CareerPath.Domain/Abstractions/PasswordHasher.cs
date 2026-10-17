using System.Security.Cryptography;
using CareerPath.Domain.Abstractions;

namespace CareerPath.Infrastructure.Security;

/// <summary>
/// PBKDF2-HMAC-SHA256 hasher with no external dependencies (.NET 6+).
/// Stored format: v1.{iterations}.{saltBase64}.{hashBase64}
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string Version = "v1";
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 600_000; // OWASP recommendation for PBKDF2-HMAC-SHA256
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"{Version}.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        if (!TryParse(hash, out var iterations, out var salt, out var expectedKey))
            return false;

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    public bool NeedsRehash(string hash) =>
        !TryParse(hash, out var iterations, out _, out _) || iterations < Iterations;

    private static bool TryParse(string hash, out int iterations, out byte[] salt, out byte[] key)
    {
        iterations = 0;
        salt = [];
        key = [];

        var parts = hash.Split('.');
        if (parts.Length != 4 || parts[0] != Version)
            return false;

        try
        {
            iterations = int.Parse(parts[1]);
            salt = Convert.FromBase64String(parts[2]);
            key = Convert.FromBase64String(parts[3]);
            return iterations > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}