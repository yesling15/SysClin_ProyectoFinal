using SysClin.Models;
using SysClin.Models.TableViewModel;
using SysClin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class ServicioController : BaseController
    {
        private readonly ISysClinContext db;

        public ServicioController()
        {
            db = new SysClinEntities();
        }

        public ServicioController(ISysClinContext context)
        {
            db = context;
        }

        /* ============================================================
           INDEX: Lista de servicios del profesional logueado
        ============================================================ */
        [HttpGet]
        public ActionResult Index()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            int idProfesional = usuarioSesion.ID_Usuario;

            var lista = db.Servicio
                .Where(s => s.ID_Usuario == idProfesional)
                .Select(s => new ServicioTableViewModel
                {
                    ID_Servicio = s.ID_Servicio,
                    ID_Usuario = s.ID_Usuario,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio
                })
                .ToList();

            return View(lista);
        }

        /* ============================================================
           CREAR - GET
        ============================================================ */
        [HttpGet]
        public ActionResult Crear()
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var model = new ServicioViewModel
            {
                ID_Usuario = usuarioSesion.ID_Usuario
            };

            return View(model);
        }

        /* ============================================================
           CREAR - POST
        ============================================================ */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ServicioViewModel model)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
                return View(model);

            var servicio = new Servicio
            {
                ID_Usuario = usuarioSesion.ID_Usuario,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Precio = model.Precio
            };

            db.Servicio.Add(servicio);
            db.SaveChanges();

            // Para mostrar SweetAlert en la vista Crear
            ViewBag.Creado = true;

            // Se devuelve la misma vista
            return View(model);
        }

        /* ============================================================
           EDITAR - GET
        ============================================================ */
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            var servicio = db.Servicio.Find(id);

            if (servicio == null || servicio.ID_Usuario != usuarioSesion.ID_Usuario)
                return HttpNotFound();

            var model = new ServicioViewModel
            {
                ID_Servicio = servicio.ID_Servicio,
                ID_Usuario = servicio.ID_Usuario,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio   // <-- Esto carga el precio correctamente
            };

            return View(model);
        }

        /* ============================================================
           EDITAR - POST
        ============================================================ */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ServicioViewModel model)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
                return View(model);

            var servicio = db.Servicio.Find(model.ID_Servicio);

            if (servicio == null || servicio.ID_Usuario != usuarioSesion.ID_Usuario)
                return HttpNotFound();

            servicio.Nombre = model.Nombre;
            servicio.Descripcion = model.Descripcion;
            servicio.Precio = model.Precio;

            db.SaveChanges();

            // Para mostrar SweetAlert en la vista Editar
            ViewBag.Editado = true;

            return View(model);
        }

        /* ============================================================
           ELIMINAR - POST (AJAX)
        ============================================================ */
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var usuarioSesion = (Usuario)Session["UsuarioLogueado"];
            if (usuarioSesion == null)
                return Content("Error");

            var servicio = db.Servicio.Find(id);

            if (servicio == null || servicio.ID_Usuario != usuarioSesion.ID_Usuario)
                return Content("Error");

            db.Servicio.Remove(servicio);
            db.SaveChanges();

            return Content("Ok");
        }
    }
}
