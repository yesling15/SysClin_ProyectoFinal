using SysClin.Models;
using SysClin.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class CitaController : BaseController
    {
        private readonly ISysClinContext db;

        public CitaController()
        {
            db = new SysClinEntities();
        }

        public CitaController(ISysClinContext context)
        {
            db = context;
        }

        // ============================================================
        // GET: Crear cita desde paciente
        // ============================================================
        [HttpGet]
        public ActionResult CrearCitaPaciente(int idProfesional, DateTime fecha, TimeSpan hora)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var profesional = db.Usuario.Find(idProfesional);
            if (profesional == null)
                return HttpNotFound();

            var servicios = db.Servicio
                .Where(s => s.ID_Usuario == idProfesional)
                .ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.ID_Servicio.ToString(),
                    Text = s.Nombre
                })
                .ToList();

            var model = new CrearCitaPacienteViewModel
            {
                ID_Paciente = usuario.ID_Usuario,
                ID_Profesional = idProfesional,
                NombreProfesional = profesional.Nombre + " " + profesional.Primer_Apellido +
                                   (string.IsNullOrEmpty(profesional.Segundo_Apellido) ? "" :
                                   " " + profesional.Segundo_Apellido),
                Fecha = fecha,
                Hora = hora,
                ServiciosDisponibles = servicios
            };

            return View("CrearCitaPaciente", model);
        }

        // ============================================================
        // POST: Crear cita desde paciente
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCitaPaciente(CrearCitaPacienteViewModel model)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                model.ServiciosDisponibles = db.Servicio
                    .Where(s => s.ID_Usuario == model.ID_Profesional)
                    .ToList()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ID_Servicio.ToString(),
                        Text = s.Nombre
                    })
                    .ToList();

                return View("CrearCitaPaciente", model);
            }

            var fecha = model.Fecha.Date;
            var hora = model.Hora;

            if (fecha < DateTime.Today || (fecha == DateTime.Today && hora <= DateTime.Now.TimeOfDay))
            {
                ModelState.AddModelError("", "No se pueden reservar citas en una fecha u hora pasada.");

                model.ServiciosDisponibles = db.Servicio
                    .Where(s => s.ID_Usuario == model.ID_Profesional)
                    .ToList()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ID_Servicio.ToString(),
                        Text = s.Nombre
                    })
                    .ToList();

                return View("CrearCitaPaciente", model);
            }

            bool existeCita = db.Cita.Any(c =>
                c.ID_Profesional == model.ID_Profesional &&
                c.Fecha == fecha &&
                c.Hora == hora &&
                c.ID_Estado_Cita == 1
            );

            if (existeCita)
            {
                ModelState.AddModelError("", "La hora seleccionada ya está ocupada.");

                model.ServiciosDisponibles = db.Servicio
                    .Where(s => s.ID_Usuario == model.ID_Profesional)
                    .ToList()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ID_Servicio.ToString(),
                        Text = s.Nombre
                    })
                    .ToList();

                return View("CrearCitaPaciente", model);
            }

            var bloqueos = db.BloqueoHorario
                .Where(b =>
                    b.ID_Usuario == model.ID_Profesional &&
                    b.Fecha_Inicio <= fecha &&
                    b.Fecha_Fin >= fecha
                )
                .ToList();

            foreach (var b in bloqueos)
            {
                DateTime inicio = b.Fecha_Inicio.Date + b.Hora_Inicio;
                DateTime fin = b.Fecha_Fin.Date + b.Hora_Fin;
                DateTime horaCita = fecha + hora;

                if (horaCita >= inicio && horaCita < fin)
                {
                    ModelState.AddModelError("", "La hora seleccionada está bloqueada por el profesional.");

                    model.ServiciosDisponibles = db.Servicio
                        .Where(s => s.ID_Usuario == model.ID_Profesional)
                        .ToList()
                        .Select(s => new SelectListItem
                        {
                            Value = s.ID_Servicio.ToString(),
                            Text = s.Nombre
                        })
                        .ToList();

                    return View("CrearCitaPaciente", model);
                }
            }

            var cita = new Cita
            {
                ID_Paciente = model.ID_Paciente.Value,
                ID_Profesional = model.ID_Profesional.Value,
                ID_Servicio = model.ID_Servicio.Value,
                ID_Estado_Cita = 1,
                Fecha = fecha,
                Hora = hora,
                Fecha_Creacion = DateTime.Now
            };

            db.Cita.Add(cita);
            db.SaveChanges();

            ViewBag.Creado = true;

            model.ServiciosDisponibles = db.Servicio
                .Where(s => s.ID_Usuario == model.ID_Profesional)
                .ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.ID_Servicio.ToString(),
                    Text = s.Nombre
                })
                .ToList();

            return View("CrearCitaPaciente", model);
        }

        // ============================================================
        // GET: Historial y próximas citas del paciente
        // ============================================================
        [HttpGet]
        public ActionResult MisCitas()
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var model = new CitasPacienteViewModel();

            model.Citas = db.Cita
                .Where(c => c.ID_Paciente == usuario.ID_Usuario)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .Select(c => new CitaItemPacienteViewModel
                {
                    ID_Cita = c.ID_Cita,
                    Fecha = c.Fecha,
                    Hora = c.Hora,
                    Profesional = c.Usuario1.Nombre + " " + c.Usuario1.Primer_Apellido,
                    Servicio = c.Servicio.Nombre,
                    Estado = c.EstadoCita.Nombre
                })
                .ToList();

            model.EstadosDisponibles = db.EstadoCita
                .ToList()
                .Select(e => new SelectListItem
                {
                    Value = e.ID_Estado_Cita.ToString(),
                    Text = e.Nombre
                })
                .ToList();

            return View("MisCitas", model);
        }

        // ============================================================
        // POST: Filtrar historial y próximas citas
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MisCitas(CitasPacienteViewModel model)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return RedirectToAction("Index", "Login");

            var query = db.Cita
                .Where(c => c.ID_Paciente == usuario.ID_Usuario);

            if (model.FechaFiltro.HasValue)
                query = query.Where(c => c.Fecha == model.FechaFiltro.Value);

            if (model.EstadoFiltro.HasValue)
                query = query.Where(c => c.ID_Estado_Cita == model.EstadoFiltro.Value);

            model.Citas = query
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .Select(c => new CitaItemPacienteViewModel
                {
                    ID_Cita = c.ID_Cita,
                    Fecha = c.Fecha,
                    Hora = c.Hora,
                    Profesional = c.Usuario1.Nombre + " " + c.Usuario1.Primer_Apellido,
                    Servicio = c.Servicio.Nombre,
                    Estado = c.EstadoCita.Nombre
                })
                .ToList();

            model.EstadosDisponibles = db.EstadoCita
                .ToList()
                .Select(e => new SelectListItem
                {
                    Value = e.ID_Estado_Cita.ToString(),
                    Text = e.Nombre
                })
                .ToList();

            return View("MisCitas", model);
        }

        // ============================================================
        // POST: Cancelar cita (PACIENTE) 
        // ============================================================
        [HttpPost]
        public ActionResult CancelarCitaPaciente(int id)
        {
            var usuario = (Usuario)Session["UsuarioLogueado"];
            if (usuario == null)
                return Content("Error");

            var cita = db.Cita.Find(id);

            if (cita == null || cita.ID_Paciente != usuario.ID_Usuario)
                return Content("Error");

            // 1. Solo citas Programadas
            if (cita.ID_Estado_Cita != 1)
                return Content("Solo se pueden cancelar citas programadas.");

            DateTime fechaHoraCita = cita.Fecha.Date + cita.Hora;

            // 2. No permitir cancelar citas pasadas
            if (fechaHoraCita <= DateTime.Now)
                return Content("No se puede cancelar una cita que ya pasó.");

            // 3. VALIDAR POLÍTICA DESDE PERFIL PROFESIONAL (SI EXISTE)
            var perfil = db.PerfilProfesional
                .FirstOrDefault(p => p.ID_Usuario == cita.ID_Profesional);

            if (perfil != null && perfil.Tiempo_Anticipacion.HasValue && !string.IsNullOrEmpty(perfil.Unidad_Anticipacion))
            {
                DateTime limite;

                if (perfil.Unidad_Anticipacion == "Horas")
                    limite = fechaHoraCita.AddHours(-perfil.Tiempo_Anticipacion.Value);
                else
                    limite = fechaHoraCita.AddDays(-perfil.Tiempo_Anticipacion.Value);

                if (DateTime.Now > limite)
                    return Content("La cita no puede cancelarse por la política del profesional.");
            }

            // 4. Cancelación lógica (NO se elimina)
            cita.ID_Estado_Cita = 2; 
            db.SaveChanges();

            return Content("Ok");
        }
    }
}