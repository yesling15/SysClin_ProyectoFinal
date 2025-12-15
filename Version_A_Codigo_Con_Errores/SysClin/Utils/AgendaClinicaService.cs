using System;
using System.Collections.Generic;
using System.Linq;
using SysClin.Models.ViewModel;
using SysClin.Models;            // Aquí viven las entidades del Model1.edmx

namespace SysClin.Utils
{
    public class AgendaClinicaService : IDisposable
    {                 
        private readonly SysClinEntities _db = new SysClinEntities();

        // ===========================
        // 3. AGENDA POR DÍA (PROFESIONAL)
        // ===========================
        public List<Cita> ObtenerAgendaProfesionalDia(int idProfesional, DateTime fecha)
        {
            var dia = fecha.Date;

            return _db.Cita
                .Where(c => c.ID_Profesional == idProfesional
                         && c.Fecha == dia)
                .OrderBy(c => c.Hora)
                .ToList();
        }

        // ===========================
        // 4. MARCAR INASISTENCIA
        // ===========================
        public void MarcarInasistencia(int idCita)
        {
            var cita = _db.Cita.FirstOrDefault(c => c.ID_Cita == idCita);
            if (cita == null)
                throw new Exception("La cita indicada no existe.");

            var estadoInasistencia = _db.EstadoCita
                .FirstOrDefault(e => e.Nombre == "Inasistencia");

            if (estadoInasistencia == null)
                throw new Exception("No existe el estado 'Inasistencia' en la tabla EstadoCita.");

            cita.ID_Estado_Cita = estadoInasistencia.ID_Estado_Cita;

            _db.SaveChanges();
        }

        // ===========================
        // 5. REPORTE DE CITAS POR PERÍODO
        // ===========================
        public List<Cita> ReporteCitasPorPeriodo(DateTime desde, DateTime hasta, int? idProfesional = null)
        {
            var query = _db.Cita
                .Where(c => c.Fecha >= desde.Date && c.Fecha <= hasta.Date);

            if (idProfesional.HasValue)
            {
                query = query.Where(c => c.ID_Profesional == idProfesional.Value);
            }

            return query
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToList();
        }

        // ===========================
        // 6. Limpieza de recursos
        // ===========================
        public void Dispose()
        {
            _db.Dispose();
        }


    }
}

