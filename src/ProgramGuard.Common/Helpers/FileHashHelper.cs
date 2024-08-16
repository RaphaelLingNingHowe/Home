using System.Security.Cryptography;

namespace ProgramGuard.Common.Helper
{
    public static class FileHashHelper
    {
        public static string ComputeSHA512Hash(string filePath)
        {
            using var sha512 = SHA512.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = sha512.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
