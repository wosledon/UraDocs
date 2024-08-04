using System.Security.Cryptography;

namespace UraDocs.ApiService.Extensions;

public static class FileExtensions
{
    public static bool TryGetFileHash(this string filePath, out string hash)
    {
        try
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fileStream);
                    hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }

            return true;
        }
        catch
        {
            hash = string.Empty;
            return false;
        }
    }
}
