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

namespace SysClin.Tests.Perfil_Profesional
{
    public class PerfilProfesionalUnitTests
    {
        /* ============================================================
           HELPER PARA MOCKEAR DbSet<T>
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
            var ctx = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }


        /* ============================================================
           CREAR CONTROLLER CON MOCKS (SIN ??=)
        ============================================================ */
        private PerfilProfesionalController CrearControllerConMocks(
            IEnumerable<PerfilProfesional> perfiles = null,
            IEnumerable<Especialidad> especialidades = null,
            IEnumerable<Usuario> usuarios = null)
        {
            if (perfiles == null)
                perfiles = new List<PerfilProfesional>();

            if (especialidades == null)
                especialidades = new List<Especialidad>
                {
                    new Especialidad { ID_Especialidad = 1, Nombre = "General" }
                };

            if (usuarios == null)
                usuarios = new List<Usuario>
                {
                    new Usuario { ID_Usuario = 1, Nombre = "Juan" }
                };

            var mockContext = new Mock<ISysClinContext>();

            mockContext.Setup(c => c.PerfilProfesional)
                .Returns(CrearDbSetMock(perfiles).Object);

            mockContext.Setup(c => c.Especialidad)
                .Returns(CrearDbSetMock(especialidades).Object);

            mockContext.Setup(c => c.Usuario)
                .Returns(CrearDbSetMock(usuarios).Object);

            return new PerfilProfesionalController(mockContext.Object);
        }


        /* ============================================================
           VALIDACIONES DEL VIEWMODEL
        ============================================================ */

        [Fact]
        public void PerfilProfesionalViewModel_Falla_CuandoEspecialidadEsNull()
        {
            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = null,
                Lugar_Atencion = "Clinica"
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("ID_Especialidad"));
        }

        [Fact]
        public void PerfilProfesionalViewModel_Falla_CuandoLugarAtencionEsVacio()
        {
            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = ""
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("Lugar_Atencion"));
        }

        [Fact]
        public void PerfilProfesionalViewModel_Falla_CuandoLugarAtencionExcedeLongitud()
        {
            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = new string('A', 150)
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("Lugar_Atencion"));
        }

        [Fact]
        public void PerfilProfesionalViewModel_Falla_CuandoLugarAtencionTieneCaracteresInvalidos()
        {
            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica!!"
            };

            var results = ValidarModelo(model);

            Assert.Contains(results, r => r.MemberNames.Contains("Lugar_Atencion"));
        }

        [Fact]
        public void PerfilProfesionalViewModel_Pasa_CuandoDatosSonValidos()
        {
            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Consultorio",
                Tiempo_Anticipacion = 24,
                Unidad_Anticipacion = "Horas"
            };

            var results = ValidarModelo(model);
            Assert.True(results.Count == 0);
        }


        /* ============================================================
           VALIDACIONES DEL CONTROLADOR – ANTICIPACIÓN
        ============================================================ */

        [Fact]
        public void PerfilProfesional_Validacion_Falla_CuandoTiempoSinUnidad()
        {
            var controller = CrearControllerConMocks();

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica",
                Tiempo_Anticipacion = 10,
                Unidad_Anticipacion = ""
            };

            controller.ModelState.Clear();
            controller.ValidarAnticipacion(model);

            Assert.True(controller.ModelState.ContainsKey("Unidad_Anticipacion"));
        }

        [Fact]
        public void PerfilProfesional_Validacion_Falla_CuandoUnidadSinTiempo()
        {
            var controller = CrearControllerConMocks();

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Consultorio",
                Tiempo_Anticipacion = null,
                Unidad_Anticipacion = "Horas"
            };

            controller.ModelState.Clear();
            controller.ValidarAnticipacion(model);

            Assert.True(controller.ModelState.ContainsKey("Tiempo_Anticipacion"));
        }

        [Fact]
        public void PerfilProfesional_Validacion_Falla_CuandoUnidadEsInvalida()
        {
            var controller = CrearControllerConMocks();

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Consultorio",
                Tiempo_Anticipacion = 5,
                Unidad_Anticipacion = "Semanas"
            };

            controller.ModelState.Clear();
            controller.ValidarAnticipacion(model);

            Assert.True(controller.ModelState.ContainsKey("Unidad_Anticipacion"));
        }

        [Fact]
        public void PerfilProfesional_Validacion_Pasa_CuandoTiempoYUnidadSonValidos()
        {
            var controller = CrearControllerConMocks();

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica",
                Tiempo_Anticipacion = 24,
                Unidad_Anticipacion = "Horas"
            };

            controller.ModelState.Clear();
            controller.ValidarAnticipacion(model);

            Assert.True(controller.ModelState.IsValid);
        }


        /* ============================================================
           CONTROLADOR – CREAR
        ============================================================ */

        [Fact]
        public void PerfilProfesional_Crear_Pasa_CuandoModeloEsValido()
        {
            var mockSet = new Mock<DbSet<PerfilProfesional>>();

            var especialidades = new List<Especialidad>
            {
                new Especialidad { ID_Especialidad = 1, Nombre = "General" }
            };

            var usuarios = new List<Usuario>
            {
                new Usuario { ID_Usuario = 10, Nombre = "Juan" }
            };

            var mockContext = new Mock<ISysClinContext>();
            mockContext.Setup(c => c.PerfilProfesional).Returns(mockSet.Object);
            mockContext.Setup(c => c.Especialidad).Returns(CrearDbSetMock(especialidades).Object);
            mockContext.Setup(c => c.Usuario).Returns(CrearDbSetMock(usuarios).Object);

            var controller = new PerfilProfesionalController(mockContext.Object);

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 10,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica X",
                Tiempo_Anticipacion = 24,
                Unidad_Anticipacion = "Horas"
            };

            controller.Crear(model);

            mockSet.Verify(s => s.Add(It.IsAny<PerfilProfesional>()), Times.Once);
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }


        [Fact]
        public void PerfilProfesional_Crear_NoGuarda_CuandoModelStateEsInvalido()
        {
            var perfiles = new List<PerfilProfesional>();
            var controller = CrearControllerConMocks(perfiles);

            controller.ModelState.AddModelError("Lugar_Atencion", "Requerido");

            controller.Crear(new PerfilProfesionalViewModel());

            Assert.Empty(perfiles);
        }

        [Fact]
        public void PerfilProfesional_Crear_NoGuarda_CuandoTiempoSinUnidad()
        {
            var perfiles = new List<PerfilProfesional>();
            var controller = CrearControllerConMocks(perfiles);

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica",
                Tiempo_Anticipacion = 10,
                Unidad_Anticipacion = ""
            };

            controller.Crear(model);

            Assert.True(controller.ModelState.ContainsKey("Unidad_Anticipacion"));
            Assert.Empty(perfiles);
        }

        [Fact]
        public void PerfilProfesional_Crear_NoGuarda_CuandoUnidadSinTiempo()
        {
            var perfiles = new List<PerfilProfesional>();
            var controller = CrearControllerConMocks(perfiles);

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = 1,
                ID_Especialidad = 1,
                Lugar_Atencion = "Clinica",
                Tiempo_Anticipacion = null,
                Unidad_Anticipacion = "Horas"
            };

            controller.Crear(model);

            Assert.True(controller.ModelState.ContainsKey("Tiempo_Anticipacion"));
            Assert.Empty(perfiles);
        }
    }
}
