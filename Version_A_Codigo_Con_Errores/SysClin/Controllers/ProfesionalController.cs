using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SysClin.Models;
using SysClin.Models.ViewModel;

namespace SysClin.Controllers
{
    public class ProfesionalController : BaseController
    {
        private readonly ISysClinContext db;

        public ProfesionalController()
        {
            db = new SysClinEntities();
        }

        public ProfesionalController(ISysClinContext context)
        {
            db = context;
        }

        // ============================================================
        // 1. BUSCAR PROFESIONALES
        // ============================================================

        [HttpGet]
        public ActionResult Buscar()
        {
            var model = new FiltroBusquedaProfesionalViewModel();
            model.Especialidades = ObtenerEspecialidades();

            return View(model);
        }

        [HttpPost]
        public ActionResult Buscar(FiltroBusquedaProfesionalViewModel filtros)
        {
            filtros.Especialidades = ObtenerEspecialidades();
            filtros.Resultados = new List<ProfesionalBusquedaViewModel>();

            // Si no hay criterios → mensaje
            if (string.IsNullOrWhiteSpace(filtros.Nombre) &&
                !filtros.ID_Especialidad.HasValue &&
                string.IsNullOrWhiteSpace(filtros.Lugar))
            {
                ViewBag.Mensaje = "Debe ingresar al menos un criterio de búsqueda.";
                return View(filtros);
            }

            string nombre = filtros.Nombre == null ? null : filtros.Nombre.Trim().ToLower();
            string lugar = filtros.Lugar == null ? null : filtros.Lugar.Trim().ToLower();

            var consulta = db.PerfilProfesional.AsQueryable();

            // FILTRO POR NOMBRE
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                consulta = consulta.Where(p =>
                    p.Usuario.Nombre.ToLower().Contains(nombre) ||
                    p.Usuario.Primer_Apellido.ToLower().Contains(nombre) ||
                    p.Usuario.Segundo_Apellido.ToLower().Contains(nombre));
            }

            // FILTRO POR ESPECIALIDAD
            if (filtros.ID_Especialidad.HasValue)
            {
                consulta = consulta.Where(p => p.ID_Especialidad == filtros.ID_Especialidad.Value);
            }

            // FILTRO POR LUGAR
            if (!string.IsNullOrWhiteSpace(lugar))
            {
                consulta = consulta.Where(p => p.Lugar_Atencion.ToLower().Contains(lugar));
            }

            // RESULTADOS
            filtros.Resultados = consulta
                .Select(p => new ProfesionalBusquedaViewModel
                {
                    ID_Usuario = p.ID_Usuario,
                    NombreCompleto = p.Usuario.Nombre + " " +
                                     p.Usuario.Primer_Apellido +
                                     (string.IsNullOrEmpty(p.Usuario.Segundo_Apellido)
                                        ? "" : " " + p.Usuario.Segundo_Apellido),
                    Especialidad = p.Especialidad.Nombre,
                    LugarAtencion = p.Lugar_Atencion
                })
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            if (!filtros.Resultados.Any())
                ViewBag.SinResultados = true;

            ViewBag.RealizoBusqueda = true;

            return View(filtros);
        }

        // ============================================================
        // 2. PERFIL DE PROFESIONAL
        // ============================================================

        [HttpGet]
        public ActionResult Perfil(int id)
        {
            var perfil = db.PerfilProfesional.FirstOrDefault(p => p.ID_Usuario == id);

            if (perfil == null)
                return HttpNotFound();

            var servicios = db.Servicio
                .Where(s => s.ID_Usuario == id)
                .Select(s => new ServicioViewModel
                {
                    ID_Servicio = s.ID_Servicio,
                    ID_Usuario = s.ID_Usuario,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio
                })
                .ToList();

            var vm = new PerfilProfesionalPublicoViewModel
            {
                ID_Usuario = perfil.ID_Usuario,
                NombreCompleto = perfil.Usuario.Nombre + " " +
                                 perfil.Usuario.Primer_Apellido +
                                 (string.IsNullOrEmpty(perfil.Usuario.Segundo_Apellido)
                                    ? "" : " " + perfil.Usuario.Segundo_Apellido),
                Especialidad = perfil.Especialidad.Nombre,
                LugarAtencion = perfil.Lugar_Atencion,
                Servicios = servicios
            };

            return View(vm);
        }

        // ============================================================
        // MÉTODOS DE APOYO
        // ============================================================

        private List<SelectListItem> ObtenerEspecialidades()
        {
            return db.Especialidad
                .OrderBy(e => e.Nombre)
                .ToList() 
                .Select(e => new SelectListItem
                {
                    Text = e.Nombre,
                    Value = e.ID_Especialidad.ToString()
                })
                .ToList();
        }
    }
}