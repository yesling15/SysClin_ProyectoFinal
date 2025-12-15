using SysClin.Models;
using SysClin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class PerfilProfesionalController : BaseController
    {
        private readonly ISysClinContext db;

        public PerfilProfesionalController()
        {
            db = new SysClinEntities();
        }

        public PerfilProfesionalController(ISysClinContext context)
        {
            db = context;
        }


        // GET: PerfilProfesional/Crear
        [HttpGet]
        public ActionResult Crear(int idUsuario)
        {
            var usuario = db.Usuario.Find(idUsuario);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            CargarCombosPerfil();

            var model = new PerfilProfesionalViewModel
            {
                ID_Usuario = idUsuario
            };

            return View(model);
        }

        // POST: PerfilProfesional/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(PerfilProfesionalViewModel model)
        {
            CargarCombosPerfil();

            ValidarAnticipacion(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var perfil = new PerfilProfesional
            {
                ID_Usuario = model.ID_Usuario,
                ID_Especialidad = model.ID_Especialidad.Value,
                Lugar_Atencion = model.Lugar_Atencion,
                Tiempo_Anticipacion = model.Tiempo_Anticipacion,
                Unidad_Anticipacion = model.Unidad_Anticipacion
            };

            db.PerfilProfesional.Add(perfil);
            db.SaveChanges();

            // MARCA para que la vista muestre el SweetAlert
            ViewBag.RegistroExitoso = true;

            // Volver a la misma vista para que aparezca el SweetAlert
            return View(model);
        }

        [HttpGet]
        public ActionResult Editar()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var perfil = db.PerfilProfesional
                           .FirstOrDefault(p => p.ID_Usuario == usuarioSesion.ID_Usuario);

            if (perfil == null)
                return RedirectToAction("Crear", new { idUsuario = usuarioSesion.ID_Usuario });

            CargarCombosPerfil();

            var model = new PerfilProfesionalViewModel
            {
                ID_Perfil_Profesional = perfil.ID_Perfil_Profesional,
                ID_Usuario = perfil.ID_Usuario,
                ID_Especialidad = perfil.ID_Especialidad,
                Lugar_Atencion = perfil.Lugar_Atencion,
                Tiempo_Anticipacion = perfil.Tiempo_Anticipacion,
                Unidad_Anticipacion = perfil.Unidad_Anticipacion
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(PerfilProfesionalViewModel model)
        {
            CargarCombosPerfil();

            // Validaciones del controlador (como hicimos con usuario)
            ValidarAnticipacion(model);

            if (!ModelState.IsValid)
                return View(model);

            var perfil = db.PerfilProfesional.Find(model.ID_Perfil_Profesional);
            if (perfil == null)
                return HttpNotFound();

            // Actualizar SOLO los campos editables
            perfil.ID_Especialidad = model.ID_Especialidad.Value;
            perfil.Lugar_Atencion = model.Lugar_Atencion;
            perfil.Tiempo_Anticipacion = model.Tiempo_Anticipacion;
            perfil.Unidad_Anticipacion = model.Unidad_Anticipacion;

            db.SaveChanges();

            // Activar SweetAlert
            ViewBag.EditarPerfilProfesionalExitoso = true;

            // Recargar modelo actualizado (para que no se borren los datos)
            var perfilActualizado = db.PerfilProfesional.Find(model.ID_Perfil_Profesional);

            var modeloRefrescado = new PerfilProfesionalViewModel
            {
                ID_Perfil_Profesional = perfilActualizado.ID_Perfil_Profesional,
                ID_Usuario = perfilActualizado.ID_Usuario,
                ID_Especialidad = perfilActualizado.ID_Especialidad,
                Lugar_Atencion = perfilActualizado.Lugar_Atencion,
                Tiempo_Anticipacion = perfilActualizado.Tiempo_Anticipacion,
                Unidad_Anticipacion = perfilActualizado.Unidad_Anticipacion
            };

            return View(modeloRefrescado);
        }

        /* ---------------- Métodos de apoyo ---------------- */

        private void CargarCombosPerfil()
        {
            ViewBag.Especialidades = new SelectList(
                db.Especialidad.ToList(),
                "ID_Especialidad",
                "Nombre");

            ViewBag.UnidadesAnticipacion = new SelectList(
                new[]
                {
                    new { Value = "", Text = "-- Seleccione --" },
                    new { Value = "Horas", Text = "Horas" },
                    new { Value = "Días",  Text = "Días"  }
                },
                "Value",
                "Text");
        }

        public void ValidarAnticipacion(PerfilProfesionalViewModel model)
        {
            bool tiempoTieneValor = model.Tiempo_Anticipacion.HasValue;
            bool unidadTieneValor = !string.IsNullOrWhiteSpace(model.Unidad_Anticipacion);

            // Si se indica un tiempo pero no la unidad
            if (tiempoTieneValor && !unidadTieneValor)
            {
                ModelState.AddModelError("Unidad_Anticipacion",
                    "Debe seleccionar una unidad de anticipación (horas o días).");
            }

            // Si se indica la unidad pero no el tiempo
            if (!tiempoTieneValor && unidadTieneValor)
            {
                ModelState.AddModelError("Tiempo_Anticipacion",
                    "Debe indicar el tiempo de anticipación.");
            }

            // Validación extra por seguridad: solo se aceptan Horas o Días si viene valor
            if (unidadTieneValor &&
                model.Unidad_Anticipacion != "Horas" &&
                model.Unidad_Anticipacion != "Días")
            {
                ModelState.AddModelError("Unidad_Anticipacion",
                    "La unidad de anticipación seleccionada no es válida.");
            }
        }
       
    }
}