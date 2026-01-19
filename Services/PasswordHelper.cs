using System.Security.Cryptography;
using System.Text;

namespace RecruitmentSystem.Services
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }
    }
}
