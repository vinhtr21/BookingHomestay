using System.Text;
using System.Security.Cryptography;


namespace Go2Hotel.Utility
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                byte[] enteredHashBytes = sha256.ComputeHash(enteredPasswordBytes);
                string enteredHashedPassword = BitConverter.ToString(enteredHashBytes).Replace("-", "").ToLower();
                return enteredHashedPassword.Equals(storedHashedPassword, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
