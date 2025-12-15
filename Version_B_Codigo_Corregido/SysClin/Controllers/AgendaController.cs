using System;
using System.Web.Mvc;
using SysClin.Models.ViewModel;
using SysClin.Models;   // Para usar la entidad Cita
using SysClin.Utils;   // Para usar AgendaClinicaService

namespace SysClin.Controllers
{
    public class AgendaController : Controller
    {
        private readonly AgendaClinicaService _agendaService = new AgendaClinicaService();             

        // =========================================================
        // 3. AGENDA DIARIA DE UN PROFESIONAL
        // =========================================================
        public ActionResult AgendaDia(int idProfesional, DateTime? fecha)
        {
            var f = fecha ?? DateTime.Today;

            var agenda = _agendaService.ObtenerAgendaProfesionalDia(idProfesional, f);

            ViewBag.IdProfesional = idProfesional;
            ViewBag.Fecha = f;

            // La vista recibirá: List<Cita>
            return View(agenda);
        }

        // =========================================================
        // 4. MARCAR INASISTENCIA
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarInasistencia(int idCita, int idProfesional, DateTime fecha)
        {
            _agendaService.MarcarInasistencia(idCita);

            // Volvemos a la agenda del día del mismo profesional y misma fecha
            return RedirectToAction(
                "AgendaDia",
                new { idProfesional = idProfesional, fecha = fecha.ToString("yyyy-MM-dd") }
            );
        }

        // =========================================================
        // 5. REPORTE DE CITAS POR PERÍODO
        // =========================================================
        public ActionResult ReporteCitas(DateTime? desde, DateTime? hasta, int? idProfesional)
        {
            // Rango por defecto: últimos 7 días
            var fechaDesde = desde ?? DateTime.Today.AddDays(-7);
            var fechaHasta = hasta ?? DateTime.Today;

            var reporte = _agendaService.ReporteCitasPorPeriodo(fechaDesde, fechaHasta, idProfesional);

            ViewBag.Desde = fechaDesde.ToString("yyyy-MM-dd");
            ViewBag.Hasta = fechaHasta.ToString("yyyy-MM-dd");
            ViewBag.IdProfesional = idProfesional;

            // La vista recibirá: List<Cita>
            return View(reporte);
        }

        // =========================================================
        // 6. Limpieza de recursos (cierra el contexto de BD del service)
        // =========================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _agendaService.Dispose();
            }
            base.Dispose(disposing);
        }        

    }
}
