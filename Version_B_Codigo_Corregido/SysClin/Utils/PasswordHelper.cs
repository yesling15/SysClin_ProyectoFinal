using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SysClin.Utils
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static bool Verify(string passwordIngresada, string hashAlmacenado)
        {
            string hashIngresada = Hash(passwordIngresada); 
            return hashIngresada == hashAlmacenado;
        }
    }
}