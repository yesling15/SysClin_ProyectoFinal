using Moq;
using SysClin.Controllers;
using SysClin.Models;
using SysClin.Models.ViewModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace SysClin.Tests.Servicios
{
    public class ServicioUnitTests
    {
        /* ============================================================
           HELPER: Crear DbSet<T> Mock
        ============================================================ */
        private Mock<DbSet<T>> CrearDbSetMock<T>(IEnumerable<T> data) where T : class
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
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }


        /* ============================================================
           SIMULACIÓN DE SESIÓN
        ============================================================ */
        private ServicioController CrearControllerConSesion(Mock<ISysClinContext> mockContext)
        {
            var controller = new ServicioController(mockContext.Object);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new MoqHttpContext(
                new Usuario { ID_Usuario = 10, Nombre = "Profesional" }
            );

            return controller;
        }


        /* ============================================================
           PRUEBAS DEL VIEWMODEL
        ============================================================ */

        [Fact]
        public void ServicioViewModel_Falla_CuandoNombreEsVacio()
        {
            var model = new ServicioViewModel
            {
                ID_Usuario = 1,
                Nombre = "",
                Precio = 1000
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("Nombre"));
        }

        [Fact]
        public void ServicioViewModel_Falla_CuandoPrecioEsNegativo()
        {
            var model = new ServicioViewModel
            {
                ID_Usuario = 1,
                Nombre = "Consulta",
                Precio = -1
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("Precio"));
        }

        [Fact]
        public void ServicioViewModel_Falla_CuandoDescripcionEsMuyLarga()
        {
            var model = new ServicioViewModel
            {
                ID_Usuario = 1,
                Nombre = "Terapia",
                Precio = 25000,
                Descripcion = new string('a', 500)
            };

            var results = ValidarModelo(model);

            Assert.True(results.Any());
        }

        [Fact]
        public void ServicioViewModel_Pasa_CuandoDatosSonValidos()
        {
            var model = new ServicioViewModel
            {
                ID_Usuario = 1,
                Nombre = "Consulta médica",
                Descripcion = "Incluye valoración",
                Precio = 15000
            };

            var results = ValidarModelo(model);

            Assert.True(results.Count == 0);
        }


        /* ============================================================
           CREAR (POST)
        ============================================================ */

        [Fact]
        public void Servicio_Crear_Falla_CuandoModelStateEsInvalido()
        {
            var mockSet = new Mock<DbSet<Servicio>>();
            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);

            var controller = CrearControllerConSesion(mockContext);
            controller.ModelState.AddModelError("Nombre", "Requerido");

            controller.Crear(new ServicioViewModel());

            mockSet.Verify(s => s.Add(It.IsAny<Servicio>()), Times.Never);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Servicio_Crear_Pasa_CuandoModeloEsValido()
        {
            var lista = new List<Servicio>();
            var mockSet = CrearDbSetMock(lista);

            mockSet.Setup(s => s.Add(It.IsAny<Servicio>()))
                   .Callback<Servicio>(s => lista.Add(s));

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);

            var controller = CrearControllerConSesion(mockContext);

            controller.Crear(new ServicioViewModel
            {
                Nombre = "Consulta",
                Descripcion = "General",
                Precio = 20000
            });

            mockSet.Verify(s => s.Add(It.IsAny<Servicio>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Servicio_Crear_AsignaCorrectamenteElIDUsuario()
        {
            var lista = new List<Servicio>();
            var mockSet = CrearDbSetMock(lista);

            mockSet.Setup(s => s.Add(It.IsAny<Servicio>()))
                   .Callback<Servicio>(s => lista.Add(s));

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);

            var controller = CrearControllerConSesion(mockContext);

            controller.Crear(new ServicioViewModel
            {
                Nombre = "Terapia",
                Precio = 30000
            });

            var agregado = lista.FirstOrDefault();

            Assert.NotNull(agregado);
            Assert.Equal(10, agregado.ID_Usuario);
        }


        /* ============================================================
           EDITAR (GET)
        ============================================================ */

        [Fact]
        public void Servicio_EditarGet_Falla_CuandoServicioNoExiste()
        {
            var mockSet = CrearDbSetMock(new List<Servicio>());
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(It.IsAny<int>())).Returns((Servicio)null);

            var controller = CrearControllerConSesion(mockContext);

            var result = controller.Editar(99);

            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void Servicio_EditarGet_Pasa_CuandoServicioExiste()
        {
            var servicios = new List<Servicio>
            {
                new Servicio { ID_Servicio = 5, ID_Usuario = 10, Nombre = "Consulta", Precio = 20000 }
            };

            var mockSet = CrearDbSetMock(servicios);
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(5)).Returns(servicios.First());

            var controller = CrearControllerConSesion(mockContext);

            var result = controller.Editar(5) as ViewResult;

            Assert.NotNull(result);
            var model = result.Model as ServicioViewModel;
            Assert.Equal("Consulta", model.Nombre);
        }


        /* ============================================================
           EDITAR (POST)
        ============================================================ */

        [Fact]
        public void Servicio_EditarPost_NoGuarda_CuandoModelStateEsInvalido()
        {
            var servicio = new Servicio
            {
                ID_Servicio = 3,
                ID_Usuario = 10,
                Nombre = "Consulta",
                Precio = 10000
            };

            var mockSet = CrearDbSetMock(new[] { servicio });
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(3)).Returns(servicio);

            var controller = CrearControllerConSesion(mockContext);
            controller.ModelState.AddModelError("Nombre", "Requerido");

            controller.Editar(new ServicioViewModel { ID_Servicio = 3 });

            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Servicio_EditarPost_Pasa_CuandoDatosSonValidos()
        {
            var servicio = new Servicio
            {
                ID_Servicio = 3,
                ID_Usuario = 10,
                Nombre = "Original",
                Precio = 10000
            };

            var mockSet = CrearDbSetMock(new[] { servicio });
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(3)).Returns(servicio);

            var controller = CrearControllerConSesion(mockContext);

            controller.Editar(new ServicioViewModel
            {
                ID_Servicio = 3,
                Nombre = "Actualizado",
                Precio = 15000
            });

            Assert.Equal("Actualizado", servicio.Nombre);
            Assert.Equal(15000, servicio.Precio);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }


        /* ============================================================
           ELIMINAR (POST)
        ============================================================ */

        [Fact]
        public void Servicio_Eliminar_Falla_CuandoNoExiste()
        {
            var mockSet = CrearDbSetMock(new List<Servicio>());
            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(It.IsAny<int>())).Returns((Servicio)null);

            var controller = CrearControllerConSesion(mockContext);

            var result = controller.Eliminar(5) as ContentResult;

            Assert.NotNull(result);
            Assert.Equal("Error", result.Content);
        }

        [Fact]
        public void Servicio_Eliminar_Pasa_CuandoExisteYEsDelUsuario()
        {
            var servicios = new List<Servicio>
            {
                new Servicio { ID_Servicio = 4, ID_Usuario = 10, Nombre = "Eliminar" }
            };

            var mockSet = CrearDbSetMock(servicios);

            mockSet.Setup(s => s.Remove(It.IsAny<Servicio>()))
                   .Callback<Servicio>(s => servicios.Remove(s));

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Servicio).Returns(mockSet.Object);
            mockContext.Setup(c => c.Servicio.Find(4)).Returns(servicios.First());

            var controller = CrearControllerConSesion(mockContext);

            var result = controller.Eliminar(4) as ContentResult;

            mockSet.Verify(s => s.Remove(It.IsAny<Servicio>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);

            Assert.Equal("Ok", result.Content);
        }
    }


    /* ============================================================
       MOCK HttpContextBase
    ============================================================ */
    public class MoqHttpContext : System.Web.HttpContextBase
    {
        private readonly IDictionary<string, object> sessionStorage;

        public MoqHttpContext(Usuario usuario)
        {
            sessionStorage = new Dictionary<string, object>();
            sessionStorage["UsuarioLogueado"] = usuario;
        }

        public override System.Web.HttpSessionStateBase Session =>
            new MoqHttpSession(sessionStorage);
    }

    public class MoqHttpSession : System.Web.HttpSessionStateBase
    {
        private readonly IDictionary<string, object> storage;

        public MoqHttpSession(IDictionary<string, object> data)
        {
            storage = data;
        }

        public override object this[string name]
        {
            get => storage.ContainsKey(name) ? storage[name] : null;
            set => storage[name] = value;
        }
    }
}
