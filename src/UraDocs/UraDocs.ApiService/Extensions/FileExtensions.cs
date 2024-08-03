using System.Security.Cryptography;

namespace UraDocs.ApiService.Extensions;

public static class FileExtensions
{
    public static string GetFileHash(this string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(fileStream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
