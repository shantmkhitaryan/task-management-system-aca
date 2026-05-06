using System.Security.Cryptography;
using System.Text;

namespace task_management_system_aca.Extensions;

public static class StringExtensions
{
    public static string HashPassword(this string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}