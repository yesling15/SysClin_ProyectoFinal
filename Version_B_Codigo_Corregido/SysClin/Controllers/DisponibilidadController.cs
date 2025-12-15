using SysClin.Models;
using SysClin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SysClin.Controllers
{
    public class DisponibilidadController : BaseController
    {
        private readonly ISysClinContext db;

        public DisponibilidadController()
        {
            db = new SysClinEntities();
        }

        public DisponibilidadController(ISysClinContext context)
        {
            db = context;
        }

        // ============================================================
        // GET: Solicitar fecha
        // ============================================================

        [HttpGet]
        public ActionResult Ver(int idProfesional, DateTime? fecha)
        {
            if (fecha == null)
                fecha = DateTime.Today;

            var model = new ConsultaDisponibilidadRequestViewModel
            {
                ID_Profesional = idProfesional,
                Fecha = fecha.Value.Date
            };

            return View(model);
        }

        // ============================================================
        // POST: Consultar disponibilidad
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ver(ConsultaDisponibilidadRequestViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var usuario = db.Usuario.Find(request.ID_Profesional);
            if (usuario == null)
            {
                ModelState.AddModelError("", "El profesional no existe.");
                return View(request);
            }

            var disponibilidad = new DisponibilidadViewModel
            {
                ID_Profesional = usuario.ID_Usuario,
                NombreProfesional = usuario.Nombre + " " + usuario.Primer_Apellido +
                                    (string.IsNullOrEmpty(usuario.Segundo_Apellido) ? "" :
                                    " " + usuario.Segundo_Apellido),
                Fecha = request.Fecha.Date
            };

            // ============================================================
            // A) Bloques laborales del día
            // ============================================================

            string nombreDia = ObtenerNombreDia(request.Fecha);

            var horarios = db.HorarioAtencion
                .Where(h => h.ID_Usuario == request.ID_Profesional &&
                            h.Dia_Semana == nombreDia)
                .ToList();

            foreach (var h in horarios)
            {
                disponibilidad.BloquesDelDia.Add(new BloqueHorarioDiaViewModel
                {
                    HoraInicio = h.Hora_Inicio,
                    HoraFin = h.Hora_Fin,
                    DuracionMinutos = h.Duracion_Minutos
                });
            }

            if (!disponibilidad.BloquesDelDia.Any())
                return View("Resultado", disponibilidad);

            // ============================================================
            // B) Citas ocupadas
            // ============================================================

            disponibilidad.HorasOcupadas = db.Cita
                .Where(c => c.ID_Profesional == request.ID_Profesional &&
                            c.Fecha == request.Fecha.Date)
                .Select(c => c.Hora)
                .ToList();

            // ============================================================
            // C) BLOQUEOS (CORREGIDO)
            // ============================================================

            var bloqueos = db.BloqueoHorario
                .Where(b => b.ID_Usuario == request.ID_Profesional &&
                            b.Fecha_Inicio <= request.Fecha &&
                            b.Fecha_Fin >= request.Fecha)
                .ToList();

            var horasBloqueadas = new List<TimeSpan>();

            foreach (var b in bloqueos)
            {
                TimeSpan inicioBloque;
                TimeSpan finBloque;

                // Caso 1: Día único (inicio y fin el mismo día)
                if (b.Fecha_Inicio.Date == request.Fecha.Date &&
                    b.Fecha_Fin.Date == request.Fecha.Date)
                {
                    inicioBloque = b.Hora_Inicio;
                    finBloque = b.Hora_Fin;
                }
                // Caso 2: Día de inicio del bloqueo
                else if (b.Fecha_Inicio.Date == request.Fecha.Date)
                {
                    inicioBloque = b.Hora_Inicio;
                    finBloque = new TimeSpan(23, 59, 0);
                }
                // Caso 3: Día intermedio (día completamente bloqueado)
                else if (request.Fecha.Date > b.Fecha_Inicio.Date &&
                         request.Fecha.Date < b.Fecha_Fin.Date)
                {
                    inicioBloque = new TimeSpan(0, 0, 0);
                    finBloque = new TimeSpan(23, 59, 0);
                }
                // Caso 4: Día de fin del bloqueo
                else if (b.Fecha_Fin.Date == request.Fecha.Date)
                {
                    inicioBloque = new TimeSpan(0, 0, 0);
                    finBloque = b.Hora_Fin;
                }
                else
                {
                    continue;
                }

                // Agregar cada minuto bloqueado al listado (slot a slot)
                var cursor = inicioBloque;
                while (cursor < finBloque)
                {
                    horasBloqueadas.Add(cursor);
                    cursor = cursor.Add(TimeSpan.FromMinutes(1));
                }
            }

            disponibilidad.HorasBloqueadas = horasBloqueadas;

            // ============================================================
            // D) Mostrar resultado
            // ============================================================

            return View("Resultado", disponibilidad);
        }

        // ============================================================
        // Mapeo del nombre del día
        // ============================================================

        private string ObtenerNombreDia(DateTime fecha)
        {
            switch (fecha.DayOfWeek)
            {
                case DayOfWeek.Monday: return "Lunes";
                case DayOfWeek.Tuesday: return "Martes";
                case DayOfWeek.Wednesday: return "Miércoles";
                case DayOfWeek.Thursday: return "Jueves";
                case DayOfWeek.Friday: return "Viernes";
                case DayOfWeek.Saturday: return "Sábado";
                case DayOfWeek.Sunday: return "Domingo";
                default: return "";
            }
        }
    }
}