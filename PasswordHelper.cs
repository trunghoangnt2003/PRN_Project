using System;
using System.Security.Cryptography;
using System.Text;

namespace PRN_Project
{
    public class PasswordHelper
    {
        public static string HashPasswordSHA1(string password)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
