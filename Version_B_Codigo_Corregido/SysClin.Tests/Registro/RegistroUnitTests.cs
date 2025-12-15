using Moq;
using SysClin.Controllers;
using SysClin.Models;
using SysClin.Models.ViewModel;
using SysClin.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace SysClin.Tests.Registro
{
    public class RegistroUnitTests
    {
        private static Mock<DbSet<T>> CrearDbSetMock<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }

        private static List<ValidationResult> ValidarModelo(object model)
        {
            var ctx = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        /* ============================================================
           PRUEBAS DEL VIEWMODEL
        ============================================================ */

        [Fact]
        public void UsuarioViewModel_Falla_CuandoTipoUsuarioEsVacio()
        {
            var model = new UsuarioViewModel { ID_Tipo_Usuario = null };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ID_Tipo_Usuario"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoTipoIdentificacionEsVacio()
        {
            var model = new UsuarioViewModel { ID_Tipo_Identificacion = null };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ID_Tipo_Identificacion"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoNumeroIdentificacionEsVacio()
        {
            var model = new UsuarioViewModel { Numero_Identificacion = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Numero_Identificacion"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoNombreEsVacio()
        {
            var model = new UsuarioViewModel { Nombre = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Nombre"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoPrimerApellidoEsVacio()
        {
            var model = new UsuarioViewModel { Primer_Apellido = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Primer_Apellido"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoCorreoEsVacio()
        {
            var model = new UsuarioViewModel { Correo_Electronico = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Correo_Electronico"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaEsVacia()
        {
            var model = new UsuarioViewModel { Contrasena = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoConfirmarContrasenaEsVacia()
        {
            var model = new UsuarioViewModel { ConfirmarContrasena = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ConfirmarContrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoTelefonoEsVacio()
        {
            var model = new UsuarioViewModel { Telefono = "" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Telefono"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoFechaNacimientoEsVacia()
        {
            var model = new UsuarioViewModel { Fecha_Nacimiento = null };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Fecha_Nacimiento"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoNombreExcedeLongitud()
        {
            var model = new UsuarioViewModel { Nombre = new string('A', 60) };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Nombre"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoNombreTieneCaracteresInvalidos()
        {
            var model = new UsuarioViewModel { Nombre = "Ana123" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Nombre"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoPrimerApellidoTieneCaracteresInvalidos()
        {
            var model = new UsuarioViewModel { Primer_Apellido = "Lopez!" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Primer_Apellido"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoSegundoApellidoTieneCaracteresInvalidos()
        {
            var model = new UsuarioViewModel { Segundo_Apellido = "Ramirez$" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Segundo_Apellido"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoCorreoExcedeLongitud()
        {
            var model = new UsuarioViewModel { Correo_Electronico = new string('a', 120) + "@mail.com" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Correo_Electronico"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoTelefonoTieneCaracteresInvalidos()
        {
            var model = new UsuarioViewModel { Telefono = "12A45678" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Telefono"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaMuyCorta()
        {
            var model = new UsuarioViewModel { Contrasena = "A1!" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaExcedeLongitud()
        {
            var model = new UsuarioViewModel { Contrasena = new string('A', 40) + "1!" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaNoTieneNumero()
        {
            var model = new UsuarioViewModel { Contrasena = "Abcdef!" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaNoTieneLetra()
        {
            var model = new UsuarioViewModel { Contrasena = "1234567!" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenaNoTieneCaracterEspecial()
        {
            var model = new UsuarioViewModel { Contrasena = "Clave123" };
            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("Contrasena"));
        }

        [Fact]
        public void UsuarioViewModel_Pasa_CuandoDatosSonValidos()
        {
            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 1,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Ana",
                Primer_Apellido = "Lopez",
                Segundo_Apellido = "Gomez",
                Correo_Electronico = "test@mail.com",
                Contrasena = "Clave123!",
                ConfirmarContrasena = "Clave123!",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-20)
            };

            var results = ValidarModelo(model);
            Assert.True(results.Count == 0);
        }

        [Fact]
        public void UsuarioViewModel_Falla_CuandoContrasenasNoCoinciden()
        {
            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 1,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Ana",
                Primer_Apellido = "Lopez",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-20),
                Correo_Electronico = "correo@mail.com",

                Contrasena = "Clave123!",
                ConfirmarContrasena = "Otra123!"
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r =>
                r.ErrorMessage != null &&
                r.ErrorMessage.Contains("no coinciden")
            );
        }

        /* ============================================================
           PRUEBAS DEL CONTROLADOR
        ============================================================ */

        private UsuarioController CrearControllerConMocks(
            IEnumerable<Usuario> usuarios = null,
            IEnumerable<TipoUsuario> tiposUsuario = null,
            IEnumerable<TipoIdentificacion> tiposIdentificacion = null)
        {
            var mockUsuarios = CrearDbSetMock(usuarios ?? new List<Usuario>());
            var mockTiposUsuario = CrearDbSetMock(tiposUsuario ?? new List<TipoUsuario>());
            var mockTiposIdentificacion = CrearDbSetMock(tiposIdentificacion ?? new List<TipoIdentificacion>());

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockUsuarios.Object);
            mockContext.Setup(c => c.TipoUsuario).Returns(mockTiposUsuario.Object);
            mockContext.Setup(c => c.TipoIdentificacion).Returns(mockTiposIdentificacion.Object);

            return new UsuarioController(mockContext.Object);
        }

        [Fact]
        public void Registro_Falla_CuandoFechaNacimientoEsHoy()
        {
            var controller = CrearControllerConMocks();
            var model = new UsuarioViewModel { Fecha_Nacimiento = DateTime.Today };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Fecha_Nacimiento"));
        }

        [Fact]
        public void Registro_Falla_CuandoCedulaNoTiene9Digitos()
        {
            var controller = CrearControllerConMocks();

            var model = new UsuarioViewModel
            {
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123"
            };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Numero_Identificacion"));
        }

        [Fact]
        public void Registro_Falla_CuandoDimexNoTieneLongitudCorrecta()
        {
            var controller = CrearControllerConMocks();

            var model = new UsuarioViewModel
            {
                ID_Tipo_Identificacion = 2,
                Numero_Identificacion = "12345"
            };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Numero_Identificacion"));
        }

        [Fact]
        public void Registro_Falla_CuandoPasaporteTieneCaracteresInvalidos()
        {
            var controller = CrearControllerConMocks();

            var model = new UsuarioViewModel
            {
                ID_Tipo_Identificacion = 3,
                Numero_Identificacion = "ABC$12"
            };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Numero_Identificacion"));
        }

        [Fact]
        public void Registro_NoConsultaBD_CuandoModelStateEsInvalido()
        {
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Usuario).Returns(CrearDbSetMock(new List<Usuario>()).Object);
            mockContext.Setup(c => c.TipoIdentificacion).Returns(CrearDbSetMock(new List<TipoIdentificacion>()).Object);
            mockContext.Setup(c => c.TipoUsuario).Returns(CrearDbSetMock(new List<TipoUsuario>()).Object);

            var controller = new UsuarioController(mockContext.Object);
            controller.ModelState.AddModelError("Nombre", "Requerido");

            var model = new UsuarioViewModel { Nombre = "" };

            controller.Registro(model);

            mockContext.Verify(c => c.Usuario, Times.Never());
        }

        [Fact]
        public void Registro_Falla_CuandoCorreoYaExiste()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Correo_Electronico = "test@mail.com" }
            };

            var controller = CrearControllerConMocks(usuarios);

            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 1,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Ana",
                Primer_Apellido = "Lopez",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-20),
                Correo_Electronico = "test@mail.com",
                Contrasena = "Clave123!",
                ConfirmarContrasena = "Clave123!"
            };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Correo_Electronico"));
        }

        [Fact]
        public void Registro_Falla_CuandoIdentificacionYaExiste()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Numero_Identificacion = "123456789" }
            };

            var controller = CrearControllerConMocks(usuarios);

            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 1,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Ana",
                Primer_Apellido = "Lopez",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-20),
                Correo_Electronico = "nuevo@mail.com",
                Contrasena = "Clave123!",
                ConfirmarContrasena = "Clave123!"
            };

            controller.Registro(model);

            Assert.True(controller.ModelState.ContainsKey("Numero_Identificacion"));
        }

        [Fact]
        public void Registro_Pasa_CuandoUsuarioEsCreado()
        {
            var mockUsuarios = CrearDbSetMock(new List<Usuario>());
            var mockTiposUsuario = CrearDbSetMock(new List<TipoUsuario>
            {
                new TipoUsuario { ID_Tipo_Usuario = 1, Nombre = "Paciente" }
            });
            var mockTiposIdentificacion = CrearDbSetMock(new List<TipoIdentificacion>
            {
                new TipoIdentificacion { ID_Tipo_Identificacion = 1, Nombre = "Cedula" }
            });

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Usuario).Returns(mockUsuarios.Object);
            mockContext.Setup(c => c.TipoUsuario).Returns(mockTiposUsuario.Object);
            mockContext.Setup(c => c.TipoIdentificacion).Returns(mockTiposIdentificacion.Object);

            var controller = new UsuarioController(mockContext.Object);

            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 1,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Ana",
                Primer_Apellido = "Lopez",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-20),
                Correo_Electronico = "nuevo@mail.com",
                Contrasena = "Clave123!",
                ConfirmarContrasena = "Clave123!"
            };

            controller.Registro(model);

            mockContext.Verify(c => c.Usuario.Add(It.IsAny<Usuario>()), Times.Once());
            mockContext.Verify(c => c.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Registro_RedireccionaAPerfilProfesional_CuandoUsuarioEsProfesional()
        {
            var tiposUsuario = new List<TipoUsuario>
            {
                new TipoUsuario { ID_Tipo_Usuario = 2, Nombre = "Profesional" }
            };

            var controller = CrearControllerConMocks(
                usuarios: new List<Usuario>(),
                tiposUsuario: tiposUsuario
            );

            var model = new UsuarioViewModel
            {
                ID_Tipo_Usuario = 2,
                ID_Tipo_Identificacion = 1,
                Numero_Identificacion = "123456789",
                Nombre = "Juan",
                Primer_Apellido = "Perez",
                Correo_Electronico = "correo@mail.com",
                Contrasena = "Clave123!",
                ConfirmarContrasena = "Clave123!",
                Telefono = "88888888",
                Fecha_Nacimiento = DateTime.Today.AddYears(-25)
            };

            var result = controller.Registro(model) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("PerfilProfesional", result.RouteValues["controller"]);
            Assert.Equal("Crear", result.RouteValues["action"]);
        }
    }
}
