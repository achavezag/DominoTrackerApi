using System.Security.Cryptography;
using DominoTracker.Application.Interfaces;

namespace DominoTracker.Infrastructure.Services;

// PBKDF2 (HMACSHA256), formato: { saltSize (4 bytes) }{ salt }{ subkey }, todo en Base64.
// No depende de ningún paquete externo de hashing: usa únicamente System.Security.Cryptography.
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int SubkeySize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SubkeySize);

        var result = new byte[SaltSize + SubkeySize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(subkey, 0, result, SaltSize, SubkeySize);

        return Convert.ToBase64String(result);
    }

    public bool Verify(string hash, string password)
    {
        var bytes = Convert.FromBase64String(hash);

        if (bytes.Length != SaltSize + SubkeySize)
            return false;

        var salt = new byte[SaltSize];
        var expectedSubkey = new byte[SubkeySize];
        Buffer.BlockCopy(bytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(bytes, SaltSize, expectedSubkey, 0, SubkeySize);

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, SubkeySize);

        return CryptographicOperations.FixedTimeEquals(expectedSubkey, actualSubkey);
    }
}
