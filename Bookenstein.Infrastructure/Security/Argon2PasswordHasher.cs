using System.Security.Cryptography;
using System.Text;
using Bookenstein.Application.Interfaces;
using Konscious.Security.Cryptography;

namespace Bookenstein.Infrastructure.Security;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    // parâmetros recomendados (ajustar conforme necessidade/infra)
    private const int SaltSize = 16;       // 128-bit
    private const int HashSize = 32;       // 256-bit
    private const int Iterations = 3;
    private const int MemoryKb = 64 * 1024; // 64 MB
    private const int DegreeOfParallelism = 2;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(password, salt);

        // formato: argon2id|m=65536,t=3,p=2|<saltB64>|<hashB64>
        return $"argon2id|m={MemoryKb},t={Iterations},p={DegreeOfParallelism}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hashString)
    {
        try
        {
            var parts = hashString.Split('|');
            if (parts.Length != 4 || !parts[0].Equals("argon2id", StringComparison.OrdinalIgnoreCase)) return false;

            var parms = parts[1].Split(',');
            int m = int.Parse(parms[0].Split('=')[1]);
            int t = int.Parse(parms[1].Split('=')[1]);
            int p = int.Parse(parms[2].Split('=')[1]);

            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);

            var actual = ComputeHash(password, salt, m, t, p);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch { return false; }
    }

    private static byte[] ComputeHash(string password, byte[] salt, int? memoryKb = null, int? iterations = null, int? dop = null)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memoryKb ?? MemoryKb,
            Iterations = iterations ?? Iterations,
            DegreeOfParallelism = dop ?? DegreeOfParallelism
        };
        return argon2.GetBytes(HashSize);
    }
}
