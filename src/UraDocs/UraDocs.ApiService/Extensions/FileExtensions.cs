using System.Security.Cryptography;
using System.Text;

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

    public static string GetFileHash(this string filePath)
    {

        if (TryGetFileHash(filePath, out var hash))
        {
            return hash;
        }

        return string.Empty;
    }

    public static string Md5(this string text)
    {
        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
