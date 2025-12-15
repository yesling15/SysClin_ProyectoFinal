using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class CitaItemPacienteViewModel
    {
        public int ID_Cita { get; set; }

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        public string Profesional { get; set; }
        public string Servicio { get; set; }

        public int ID_Estado_Cita { get; set; }
        public string Estado { get; set; }

        // Para saber si mostrar botones
        public bool PuedeCancelar { get; set; }
        public bool PuedeReprogramar { get; set; }
    }
}