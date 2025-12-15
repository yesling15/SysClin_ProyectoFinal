using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class CitaTableViewModel
    {
        public int ID_Cita { get; set; }
        public int ID_Paciente { get; set; }
        public int ID_Profesional { get; set; }
        public int ID_Servicio { get; set; }
        public int ID_Estado_Cita { get; set; }
        public System.DateTime Fecha { get; set; }
        public System.TimeSpan Hora { get; set; }
        public System.DateTime Fecha_Creacion { get; set; }
        public int? ID_Cita_Original { get; set; }
    }
}