using SysClin.Models;
using SysClin.Models.TableViewModel;
using SysClin.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class BloqueoHorarioController : BaseController
    {
        private readonly ISysClinContext db;

        public BloqueoHorarioController()
        {
            db = new SysClinEntities();
        }

        public BloqueoHorarioController(ISysClinContext context)
        {
            db = context;
        }

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        public ActionResult Index()
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            int idProfesional = usuario.ID_Usuario;

            var lista = db.BloqueoHorario
                .Where(b => b.ID_Usuario == idProfesional)
                .OrderBy(b => b.Fecha_Inicio)
                .ThenBy(b => b.Hora_Inicio)
                .Select(b => new BloqueoHorarioTableViewModel
                {
                    ID_Bloqueo_Horario = b.ID_Bloqueo_Horario,
                    ID_Usuario = b.ID_Usuario,
                    Fecha_Inicio = b.Fecha_Inicio,
                    Hora_Inicio = b.Hora_Inicio,
                    Fecha_Fin = b.Fecha_Fin,
                    Hora_Fin = b.Hora_Fin
                })
                .ToList();

            return View(lista);
        }

        // ============================================================
        // CREAR – GET
        // ============================================================
        [HttpGet]
        public ActionResult Crear()
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            return View(new BloqueoHorarioViewModel
            {
                ID_Usuario = usuario.ID_Usuario,
                Fecha_Inicio = DateTime.Today,
                Fecha_Fin = DateTime.Today
            });
        }

        // ============================================================
        // CREAR – POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(BloqueoHorarioViewModel model)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
                return View(model);

            // Construir DateTime completos
            DateTime inicio = model.Fecha_Inicio.Date + model.Hora_Inicio;
            DateTime fin = model.Fecha_Fin.Date + model.Hora_Fin;

            if (fin <= inicio)
            {
                ModelState.AddModelError("", "La fecha y hora de fin deben ser mayores que la fecha y hora de inicio.");
                return View(model);
            }

            if (ExisteSolapamientoBloqueos(model, model.ID_Usuario))
            {
                ModelState.AddModelError("", "Este bloqueo se solapa con otro bloqueo existente.");
                return View(model);
            }

            if (ExisteCitaEnRango(model, model.ID_Usuario))
            {
                ModelState.AddModelError("", "No se puede bloquear este rango porque existen citas programadas.");
                return View(model);
            }

            db.BloqueoHorario.Add(new BloqueoHorario
            {
                ID_Usuario = model.ID_Usuario,
                Fecha_Inicio = model.Fecha_Inicio,
                Hora_Inicio = model.Hora_Inicio,
                Fecha_Fin = model.Fecha_Fin,
                Hora_Fin = model.Hora_Fin
            });

            db.SaveChanges();
            ViewBag.Creado = true;

            return View(model);
        }

        // ============================================================
        // EDITAR – GET
        // ============================================================
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var bloqueo = db.BloqueoHorario.Find(id);

            if (bloqueo == null || bloqueo.ID_Usuario != usuario.ID_Usuario)
                return HttpNotFound();

            return View(new BloqueoHorarioViewModel
            {
                ID_Bloqueo_Horario = bloqueo.ID_Bloqueo_Horario,
                ID_Usuario = bloqueo.ID_Usuario,
                Fecha_Inicio = bloqueo.Fecha_Inicio,
                Hora_Inicio = bloqueo.Hora_Inicio,
                Fecha_Fin = bloqueo.Fecha_Fin,
                Hora_Fin = bloqueo.Hora_Fin
            });
        }

        // ============================================================
        // EDITAR – POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(BloqueoHorarioViewModel model)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
                return View(model);

            DateTime inicio = model.Fecha_Inicio.Date + model.Hora_Inicio;
            DateTime fin = model.Fecha_Fin.Date + model.Hora_Fin;

            if (fin <= inicio)
            {
                ModelState.AddModelError("", "La fecha y hora de fin deben ser mayores que la fecha y hora de inicio.");
                return View(model);
            }

            if (ExisteSolapamientoBloqueos(model, model.ID_Usuario, model.ID_Bloqueo_Horario))
            {
                ModelState.AddModelError("", "Este bloqueo se solapa con otro bloqueo existente.");
                return View(model);
            }

            if (ExisteCitaEnRango(model, model.ID_Usuario))
            {
                ModelState.AddModelError("", "No se puede bloquear este rango porque existen citas programadas.");
                return View(model);
            }

            var bloqueo = db.BloqueoHorario.Find(model.ID_Bloqueo_Horario);

            bloqueo.Fecha_Inicio = model.Fecha_Inicio;
            bloqueo.Hora_Inicio = model.Hora_Inicio;
            bloqueo.Fecha_Fin = model.Fecha_Fin;
            bloqueo.Hora_Fin = model.Hora_Fin;

            db.SaveChanges();

            ViewBag.Editado = true;
            return View(model);
        }

        // ============================================================
        // ELIMINAR – POST
        // ============================================================
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return Content("Error");

            var bloqueo = db.BloqueoHorario.Find(id);

            if (bloqueo == null || bloqueo.ID_Usuario != usuario.ID_Usuario)
                return Content("Error");

            db.BloqueoHorario.Remove(bloqueo);
            db.SaveChanges();

            return Content("Ok");
        }

        // ============================================================
        // MÉTODOS DE APOYO
        // ============================================================

        private bool ExisteSolapamientoBloqueos(BloqueoHorarioViewModel model, int idUsuario, int? idExcluir = null)
        {
            DateTime inicioNuevo = model.Fecha_Inicio.Date + model.Hora_Inicio;
            DateTime finNuevo = model.Fecha_Fin.Date + model.Hora_Fin;

            // 1. Traer bloqueos que puedan chocar 
            var bloqueos = db.BloqueoHorario
                .Where(b =>
                    b.ID_Usuario == idUsuario &&
                    (idExcluir == null || b.ID_Bloqueo_Horario != idExcluir) &&
                    b.Fecha_Fin >= model.Fecha_Inicio &&
                    b.Fecha_Inicio <= model.Fecha_Fin
                )
                .ToList(); 

            // 2. Validar solapamientos en memoria usando DateTime completo
            foreach (var b in bloqueos)
            {
                DateTime inicioExistente = b.Fecha_Inicio.Date + b.Hora_Inicio;
                DateTime finExistente = b.Fecha_Fin.Date + b.Hora_Fin;

                if (inicioExistente < finNuevo && finExistente > inicioNuevo)
                    return true;
            }

            return false;
        }

        private bool ExisteCitaEnRango(BloqueoHorarioViewModel model, int idUsuario)
        {
            DateTime inicioNuevo = model.Fecha_Inicio.Date + model.Hora_Inicio;
            DateTime finNuevo = model.Fecha_Fin.Date + model.Hora_Fin;

            var citas = db.Cita
                .Where(c =>
                    c.ID_Profesional == idUsuario &&
                    c.Fecha >= model.Fecha_Inicio &&
                    c.Fecha <= model.Fecha_Fin
                )
                .ToList();

            foreach (var c in citas)
            {
                DateTime inicioCita = c.Fecha.Date + c.Hora;
                DateTime finCita = inicioCita.AddMinutes(1); // Las citas no tienen duración, así que se trata como un punto

                if (inicioCita < finNuevo && finCita > inicioNuevo)
                    return true;
            }

            return false;
        }
    }
}