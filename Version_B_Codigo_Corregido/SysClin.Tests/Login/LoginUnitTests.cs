using Moq;
using SysClin.Controllers;
using SysClin.Models;
using SysClin.Models.ViewModel;
using SysClin.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace SysClin.Tests.Login
{
    public class LoginUnitTests
    {
        // Crea un DbSet mockeado para simular consultas LINQ sobre Usuario.
        private static Mock<DbSet<Usuario>> CrearDbSetMock(IEnumerable<Usuario> data)
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<Usuario>>();
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }

        /* ============================================================
           PRUEBAS DEL VIEWMODEL
        ============================================================ */

        [Fact]
        public void LoginViewModel_Falla_CuandoCorreoEsVacio()
        {
            var model = new LoginViewModel
            {
                Correo = "",
                Contrasena = "algo"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(model, context, results, true);

            Assert.False(valido);
            Assert.Contains(results, r => r.MemberNames.Contains("Correo"));
        }

        [Fact]
        public void LoginViewModel_Falla_CuandoCorreoTieneFormatoInvalido()
        {
            var model = new LoginViewModel
            {
                Correo = "correo-invalido",
                Contrasena = "abc123"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(model, context, results, true);

            Assert.False(valido);
            Assert.Contains(results, r => r.MemberNames.Contains("Correo"));
        }

        [Fact]
        public void LoginViewModel_Falla_CuandoContrasenaEsVacia()
        {
            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = ""
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(model, context, results, true);

            Assert.False(valido);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void LoginViewModel_Pasa_CuandoDatosSonValidos()
        {
            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = "Clave123!"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            bool valido = Validator.TryValidateObject(model, context, results, true);

            Assert.True(valido);
        }

        /* ============================================================
           PRUEBAS DEL CONTROLADOR
        ============================================================ */

        // Usuario NO existe - Login falla
        [Fact]
        public void Login_Falla_CuandoUsuarioNoExiste()
        {
            var mockSet = CrearDbSetMock(new List<Usuario>());
            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

            var controller = new LoginController(mockContext.Object);

            var model = new LoginViewModel
            {
                Correo = "noexiste@correo.com",
                Contrasena = "123"
            };

            var result = controller.Index(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ViewBag.LoginError);
        }

        // Usuario existe, pero contraseña es incorrecta
        [Fact]
        public void Login_Falla_CuandoContrasenaEsIncorrecta()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Correo_Electronico = "test@correo.com",
                    Contrasena = PasswordHelper.Hash("ClaveCorrecta")
                }
            };

            var mockSet = CrearDbSetMock(usuarios);
            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

            var controller = new LoginController(mockContext.Object);

            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = "ClaveMala"
            };

            var result = controller.Index(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ViewBag.LoginError);
        }

        // Login correcto - redirige a Home
        [Fact]
        public void Login_Exitoso_RedireccionaAlHome()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Correo_Electronico = "test@correo.com",
                    Contrasena = PasswordHelper.Hash("ClaveSegura1")
                }
            };

            var mockSet = CrearDbSetMock(usuarios);
            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

            var controller = new LoginController(mockContext.Object);

            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = "ClaveSegura1"
            };

            var result = controller.Index(model) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.RouteValues["controller"]);
            Assert.Equal("Index", result.RouteValues["action"]);
        }

        // Modelo inválido - no consulta BD
        [Fact]
        public void Login_NoConsultaBD_SiModeloEsInvalido()
        {
            var mockSet = CrearDbSetMock(new List<Usuario>());
            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockSet.Object);

            var controller = new LoginController(mockContext.Object);

            controller.ModelState.AddModelError("Correo", "Requerido");

            var model = new LoginViewModel
            {
                Correo = "",
                Contrasena = ""
            };

            var result = controller.Index(model) as ViewResult;

            Assert.NotNull(result);
            mockContext.Verify(c => c.Usuario, Times.Never());
        }
    }
}
