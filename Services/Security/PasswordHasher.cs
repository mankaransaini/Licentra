using BCrypt.Net;
using Licentra.API.Interfaces.Security;

namespace Licentra.API.Services.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
                return false;

            if (passwordHash.StartsWith("$2a$") || passwordHash.StartsWith("$2b$") || passwordHash.StartsWith("$2y$") || passwordHash.StartsWith("$2x$"))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(password, passwordHash);
                }
                catch
                {
                    return false;
                }
            }

            // Fallback for plain text stored passwords in database
            return password == passwordHash;
        }
    }
}