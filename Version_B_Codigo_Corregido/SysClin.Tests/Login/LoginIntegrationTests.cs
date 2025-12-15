using SysClin.Controllers;
using SysClin.Models;
using SysClin.Models.ViewModel;
using SysClin.Utils;
using System.Web.Mvc;
using Xunit;

namespace SysClin.Tests.Login
{
    public class LoginIntegrationTests
    {
        [Fact]
        public void Login_Integracion_Exitoso_EjecutaFlujo()
        {
            // Arrange
            var context = new FakeSysClinContext();

            context.UsuariosData.Add(new Usuario
            {
                Correo_Electronico = "test@correo.com",
                Contrasena = PasswordHelper.Hash("ClaveSegura1")
            });

            var controller = new LoginController(context);

            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = "ClaveSegura1"
            };

            // Act
            var result = controller.Index(model);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Login_Integracion_Falla_Cuando_Usuario_No_Existe()
        {
            var context = new FakeSysClinContext();
            var controller = new LoginController(context);

            var model = new LoginViewModel
            {
                Correo = "noexiste@correo.com",
                Contrasena = "123"
            };

            var result = controller.Index(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ViewBag.LoginError);
        }

        [Fact]
        public void Login_Integracion_Falla_Cuando_Contrasena_Es_Incorrecta()
        {
            var context = new FakeSysClinContext();

            context.UsuariosData.Add(new Usuario
            {
                Correo_Electronico = "test@correo.com",
                Contrasena = PasswordHelper.Hash("ClaveBuena")
            });

            var controller = new LoginController(context);

            var model = new LoginViewModel
            {
                Correo = "test@correo.com",
                Contrasena = "ClaveMala"
            };

            var result = controller.Index(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ViewBag.LoginError);
        }
    }
}