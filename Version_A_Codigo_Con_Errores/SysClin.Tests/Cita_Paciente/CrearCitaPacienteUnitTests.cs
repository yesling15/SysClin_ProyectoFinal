using Moq;
using SysClin.Controllers;
using SysClin.Models;
using SysClin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Xunit;

namespace SysClin.Tests.Cita_Paciente
{
    public class CrearCitaPacienteUnitTests
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
        private CitaController CrearControllerConSesion(Mock<ISysClinContext> mockContext)
        {
            var controller = new CitaController(mockContext.Object);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new MoqHttpContext(
                new Usuario { ID_Usuario = 20, Nombre = "Paciente" }
            );

            return controller;
        }

        /* ============================================================
           PRUEBAS DEL VIEWMODEL
        ============================================================ */

        [Fact]
        public void CrearCitaPacienteViewModel_Falla_CuandoIDProfesionalEsNull()
        {
            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = null,
                ID_Paciente = 1,
                Fecha = DateTime.Today,
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 1
            };

            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ID_Profesional"));
        }

        [Fact]
        public void CrearCitaPacienteViewModel_Falla_CuandoIDPacienteEsNull()
        {
            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = null,
                Fecha = DateTime.Today,
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 1
            };

            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ID_Paciente"));
        }

        [Fact]
        public void CrearCitaPacienteViewModel_Falla_CuandoIDServicioEsNull()
        {
            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 2,
                Fecha = DateTime.Today,
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = null
            };

            var results = ValidarModelo(model);
            Assert.Contains(results, r => r.MemberNames.Contains("ID_Servicio"));
        }

        [Fact]
        public void CrearCitaPacienteViewModel_Pasa_CuandoEsValido()
        {
            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 2,
                Fecha = DateTime.Today.AddDays(1),
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 5
            };

            var results = ValidarModelo(model);
            Assert.True(results.Count == 0);
        }

        /* ============================================================
           CREAR CITA: POST
        ============================================================ */

        [Fact]
        public void CrearCitaPaciente_Falla_CuandoModelStateEsInvalido()
        {
            var mockCitas = CrearDbSetMock(new List<Cita>());
            var mockServicios = CrearDbSetMock(new List<Servicio>());
            var mockBloqueos = CrearDbSetMock(new List<BloqueoHorario>());

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Cita).Returns(mockCitas.Object);
            mockContext.Setup(c => c.Servicio).Returns(mockServicios.Object);
            mockContext.Setup(c => c.BloqueoHorario).Returns(mockBloqueos.Object);

            var controller = CrearControllerConSesion(mockContext);
            controller.ModelState.AddModelError("ID_Servicio", "Requerido");

            var result = controller.CrearCitaPaciente(new CrearCitaPacienteViewModel());

            Assert.IsType<ViewResult>(result);
            mockContext.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Fact]
        public void CrearCitaPaciente_Falla_CuandoFechaEsPasada()
        {
            var mockCitas = CrearDbSetMock(new List<Cita>());
            var mockServicios = CrearDbSetMock(new List<Servicio>());
            var mockBloqueos = CrearDbSetMock(new List<BloqueoHorario>());

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Cita).Returns(mockCitas.Object);
            mockContext.Setup(c => c.Servicio).Returns(mockServicios.Object);
            mockContext.Setup(c => c.BloqueoHorario).Returns(mockBloqueos.Object);

            var controller = CrearControllerConSesion(mockContext);

            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 20,
                Fecha = DateTime.Today.AddDays(-1),
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 2
            };

            var result = controller.CrearCitaPaciente(model) as ViewResult;

            Assert.True(controller.ModelState.Count > 0);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CrearCitaPaciente_Falla_CuandoHoraEstaOcupada()
        {
            var citas = new List<Cita>
            {
                new Cita
                {
                    ID_Profesional = 1,
                    Fecha = DateTime.Today.AddDays(1),
                    Hora = new TimeSpan(10,0,0),
                    ID_Estado_Cita = 1
                }
            };

            var mockCitas = CrearDbSetMock(citas);
            var mockServicios = CrearDbSetMock(new List<Servicio>());
            var mockBloqueos = CrearDbSetMock(new List<BloqueoHorario>());

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Cita).Returns(mockCitas.Object);
            mockContext.Setup(c => c.Servicio).Returns(mockServicios.Object);
            mockContext.Setup(c => c.BloqueoHorario).Returns(mockBloqueos.Object);

            var controller = CrearControllerConSesion(mockContext);

            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 20,
                Fecha = DateTime.Today.AddDays(1),
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 5
            };

            controller.CrearCitaPaciente(model);

            Assert.True(controller.ModelState.Count > 0);
        }

        [Fact]
        public void CrearCitaPaciente_Falla_CuandoHorarioEstaBloqueado()
        {
            var bloqueos = new List<BloqueoHorario>
            {
                new BloqueoHorario
                {
                    ID_Usuario = 1,
                    Fecha_Inicio = DateTime.Today.AddDays(1),
                    Fecha_Fin = DateTime.Today.AddDays(1),
                    Hora_Inicio = new TimeSpan(9,0,0),
                    Hora_Fin = new TimeSpan(11,0,0)
                }
            };

            var mockCitas = CrearDbSetMock(new List<Cita>());
            var mockServicios = CrearDbSetMock(new List<Servicio>());
            var mockBloqueos = CrearDbSetMock(bloqueos);

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Cita).Returns(mockCitas.Object);
            mockContext.Setup(c => c.Servicio).Returns(mockServicios.Object);
            mockContext.Setup(c => c.BloqueoHorario).Returns(mockBloqueos.Object);

            var controller = CrearControllerConSesion(mockContext);

            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 20,
                Fecha = DateTime.Today.AddDays(1),
                Hora = new TimeSpan(10, 0, 0),
                ID_Servicio = 5
            };

            controller.CrearCitaPaciente(model);

            Assert.True(controller.ModelState.Count > 0);
        }

        [Fact]
        public void CrearCitaPaciente_Pasa_CuandoTodoEsValido()
        {
            var lista = new List<Cita>();

            var mockCitas = CrearDbSetMock(lista);
            mockCitas.Setup(s => s.Add(It.IsAny<Cita>()))
                     .Callback<Cita>(c => lista.Add(c));

            var mockServicios = CrearDbSetMock(new List<Servicio>());
            var mockBloqueos = CrearDbSetMock(new List<BloqueoHorario>());

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.Cita).Returns(mockCitas.Object);
            mockContext.Setup(c => c.Servicio).Returns(mockServicios.Object);
            mockContext.Setup(c => c.BloqueoHorario).Returns(mockBloqueos.Object);

            var controller = CrearControllerConSesion(mockContext);

            var model = new CrearCitaPacienteViewModel
            {
                ID_Profesional = 1,
                ID_Paciente = 20,
                Fecha = DateTime.Today.AddDays(2),
                Hora = new TimeSpan(14, 0, 0),
                ID_Servicio = 3
            };

            controller.CrearCitaPaciente(model);

            Assert.Single(lista);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }


    /* ============================================================
       MOCK HttpContextBase (MISMO ESTILO QUE EN SERVICIO)
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