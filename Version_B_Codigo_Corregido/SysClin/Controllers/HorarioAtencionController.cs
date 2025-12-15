using SysClin.Models;
using SysClin.Models.TableViewModel;
using SysClin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class HorarioAtencionController : BaseController
    {
        private readonly ISysClinContext db;

        public HorarioAtencionController()
        {
            db = new SysClinEntities();
        }

        public HorarioAtencionController(ISysClinContext context)
        {
            db = context;
        }

        /* ============================================================
           INDEX – Lista agrupada por día (sin cambios)
        ============================================================ */
        [HttpGet]
        public ActionResult Index()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            int idProfesional = usuarioSesion.ID_Usuario;

            var horarios = db.HorarioAtencion
                .Where(h => h.ID_Usuario == idProfesional)
                .ToList();

            var lista = horarios
                .Select(h => new HorarioAtencionTableViewModel
                {
                    ID_Horario = h.ID_Horario,
                    Dia_Semana = h.Dia_Semana,
                    Hora_Inicio = h.Hora_Inicio.ToString(@"hh\:mm"),
                    Hora_Fin = h.Hora_Fin.ToString(@"hh\:mm"),
                    Duracion_Minutos = h.Duracion_Minutos
                })
                .ToList();

            return View(lista);
        }

        /* ============================================================
           CREAR – GET
        ============================================================ */
        [HttpGet]
        public ActionResult Crear()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var model = new HorarioAtencionViewModel
            {
                ID_Usuario = usuarioSesion.ID_Usuario
            };

            ViewBag.Duraciones = ObtenerDuraciones();
            ViewBag.DiasSemana = ObtenerDiasSemana();

            return View(model);
        }

        /* ============================================================
           CREAR – POST
        ============================================================ */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(HorarioAtencionViewModel model)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Duraciones = ObtenerDuraciones();
            ViewBag.DiasSemana = ObtenerDiasSemana();

            if (!ModelState.IsValid)
                return View(model);

            if (model.Hora_Inicio >= model.Hora_Fin)
            {
                ModelState.AddModelError("", "La hora de inicio debe ser menor que la hora de fin.");
                return View(model);
            }

            /* NUEVA VALIDACIÓN: duración exacta dentro del bloque */
            var totalMinutos = (int)(model.Hora_Fin - model.Hora_Inicio).TotalMinutes;

            if (totalMinutos % model.Duracion_Minutos != 0)
            {
                ModelState.AddModelError("",
                    "El rango entre la hora de inicio y la hora de fin debe dividirse exactamente según la duración seleccionada.");
                return View(model);
            }

            if (ExisteSolapamiento(model, model.ID_Usuario))
            {
                ModelState.AddModelError("", "Este horario se solapa con uno ya registrado.");
                return View(model);
            }

            var horario = new HorarioAtencion
            {
                ID_Usuario = model.ID_Usuario,
                Dia_Semana = model.Dia_Semana,
                Hora_Inicio = model.Hora_Inicio,
                Hora_Fin = model.Hora_Fin,
                Duracion_Minutos = model.Duracion_Minutos
            };

            db.HorarioAtencion.Add(horario);
            db.SaveChanges();

            ViewBag.Creado = true;
            return View(model);
        }

        /* ============================================================
           EDITAR – GET
        ============================================================ */
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var horario = db.HorarioAtencion.Find(id);

            if (horario == null || horario.ID_Usuario != usuarioSesion.ID_Usuario)
                return HttpNotFound();

            var model = new HorarioAtencionViewModel
            {
                ID_Horario = horario.ID_Horario,
                ID_Usuario = horario.ID_Usuario,
                Dia_Semana = horario.Dia_Semana,
                Hora_Inicio = horario.Hora_Inicio,
                Hora_Fin = horario.Hora_Fin,
                Duracion_Minutos = horario.Duracion_Minutos
            };

            ViewBag.Duraciones = ObtenerDuraciones(model.Duracion_Minutos);
            ViewBag.DiasSemana = ObtenerDiasSemana(model.Dia_Semana);

            return View(model);
        }

        /* ============================================================
           EDITAR – POST
        ============================================================ */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(HorarioAtencionViewModel model)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Duraciones = ObtenerDuraciones(model.Duracion_Minutos);
            ViewBag.DiasSemana = ObtenerDiasSemana(model.Dia_Semana);

            if (!ModelState.IsValid)
                return View(model);

            if (model.Hora_Inicio >= model.Hora_Fin)
            {
                ModelState.AddModelError("", "La hora de inicio debe ser menor que la hora de fin.");
                return View(model);
            }

            /* NUEVA VALIDACIÓN: duración exacta dentro del bloque */
            var totalMinutos = (int)(model.Hora_Fin - model.Hora_Inicio).TotalMinutes;

            if (totalMinutos % model.Duracion_Minutos != 0)
            {
                ModelState.AddModelError("",
                    "El rango entre la hora de inicio y la hora de fin debe dividirse exactamente según la duración seleccionada.");
                return View(model);
            }

            if (ExisteSolapamiento(model, model.ID_Usuario, model.ID_Horario))
            {
                ModelState.AddModelError("", "Este horario se solapa con uno ya registrado.");
                return View(model);
            }

            var horario = db.HorarioAtencion.Find(model.ID_Horario);

            horario.Dia_Semana = model.Dia_Semana;
            horario.Hora_Inicio = model.Hora_Inicio;
            horario.Hora_Fin = model.Hora_Fin;
            horario.Duracion_Minutos = model.Duracion_Minutos;

            db.SaveChanges();

            ViewBag.Editado = true;
            return View(model);
        }

        /* ============================================================
           ELIMINAR – POST
        ============================================================ */
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return Content("Error");

            var horario = db.HorarioAtencion.Find(id);

            if (horario == null || horario.ID_Usuario != usuarioSesion.ID_Usuario)
                return Content("Error");

            db.HorarioAtencion.Remove(horario);
            db.SaveChanges();

            return Content("Ok");
        }

        /* ============================================================
           MÉTODOS DE APOYO
        ============================================================ */

        private bool ExisteSolapamiento(HorarioAtencionViewModel model, int idUsuario, int? idExcluir = null)
        {
            return db.HorarioAtencion.Any(h =>
                h.ID_Usuario == idUsuario &&
                h.Dia_Semana == model.Dia_Semana &&
                (idExcluir == null || h.ID_Horario != idExcluir) &&
                h.Hora_Inicio < model.Hora_Fin &&
                h.Hora_Fin > model.Hora_Inicio
            );
        }

        private List<SelectListItem> ObtenerDuraciones(int? seleccionada = null)
        {
            var duraciones = new[] { 15, 20, 30, 45, 60 };

            return duraciones.Select(d => new SelectListItem
            {
                Text = $"{d} minutos",
                Value = d.ToString(),
                Selected = seleccionada.HasValue && seleccionada.Value == d
            }).ToList();
        }

        private List<SelectListItem> ObtenerDiasSemana(string seleccionada = null)
        {
            string[] dias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            return dias.Select(d => new SelectListItem
            {
                Text = d,
                Value = d,
                Selected = (seleccionada != null && seleccionada == d)
            }).ToList();
        }
    }
}
