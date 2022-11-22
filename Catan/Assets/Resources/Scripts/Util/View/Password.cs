using System.Security.Cryptography;
using System.Text;

namespace Util.View
{
    public static class Password
    {
        public static string HashPassword(string password)
        { 
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return Encoding.Default.GetString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
