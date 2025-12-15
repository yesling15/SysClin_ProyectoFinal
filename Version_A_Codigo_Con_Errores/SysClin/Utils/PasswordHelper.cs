using System;
using System.Security.Cryptography;
using System.Text;

namespace SysClin.Utils
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var texto = password + DateTime.Now.Ticks.ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
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
            var hashIngresada = Hash(passwordIngresada + " ");
            return hashIngresada == hashAlmacenado;
        }
    }
}
