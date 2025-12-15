using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysClin.Models.ViewModel
{
    public class CitasPacienteViewModel
    {
        public List<CitaItemPacienteViewModel> Citas { get; set; }
        = new List<CitaItemPacienteViewModel>();

        // Para filtros
        public DateTime? FechaFiltro { get; set; }
        public int? EstadoFiltro { get; set; }

        // Para combo de estados de cita
        public List<SelectListItem> EstadosDisponibles { get; set; }
            = new List<SelectListItem>();
    }
}