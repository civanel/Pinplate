using Konscious.Security.Cryptography;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pinplate
{
    public static class PasswordHelper
    {
        private const int Parallelism = 1;
        private const int Iterations = 3;
        private const int MemorySizeKb = 65536;

        public static bool VerifyPassword(string pass, string hPass)
        {
            if (string.IsNullOrWhiteSpace(pass) || string.IsNullOrEmpty(hPass))
                return false;

            byte[] combined;
            try
            {
                combined = Convert.FromBase64String(hPass);
            }
            catch
            {
                return false;
            }

            if (combined.Length < 16 + 32)
                return false;

            var salt = combined.Take(16).ToArray();
            var originalHash = combined.Skip(16).Take(32).ToArray();

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pass))
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySizeKb
            };

            var newHash = argon2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }

        public static string HashingPassword(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass))
                throw new ArgumentException("Пароль пустой!");

            var salt = RandomNumberGenerator.GetBytes(16);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pass))
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                Iterations = Iterations,
                MemorySize = MemorySizeKb
            };

            var hash = argon2.GetBytes(32);
            var comb = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, comb, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, comb, salt.Length, hash.Length);

            return Convert.ToBase64String(comb);
        }
    }
}