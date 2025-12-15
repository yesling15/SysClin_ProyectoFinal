using SysClin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SysClin.Tests.Utils
{
    public class PasswordHelperTests
    {
        [Fact]
        public void Hash_GeneraMismoHash_ParaMismaContrasena()
        {
            // Arrange
            string password = "MiPassword123!";

            // Act
            string hash1 = PasswordHelper.Hash(password);
            string hash2 = PasswordHelper.Hash(password);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Hash_GeneraHashDistinto_ParaContrasenasDiferentes()
        {
            // Arrange
            string password1 = "Password123!";
            string password2 = "Password123?";

            // Act
            string hash1 = PasswordHelper.Hash(password1);
            string hash2 = PasswordHelper.Hash(password2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void Verify_RetornaTrue_CuandoLaContrasenaEsCorrecta()
        {
            // Arrange
            string password = "ClaveSegura1@";
            string hashAlmacenado = PasswordHelper.Hash(password);

            // Act
            bool resultado = PasswordHelper.Verify("ClaveSegura1@", hashAlmacenado);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void Verify_RetornaFalse_CuandoLaContrasenaEsIncorrecta()
        {
            // Arrange
            string passwordCorrecta = "ClaveSegura1@";
            string hashAlmacenado = PasswordHelper.Hash(passwordCorrecta);

            // Act
            bool resultado = PasswordHelper.Verify("ClaveErronea!", hashAlmacenado);

            // Assert
            Assert.False(resultado);
        }
    }
}
