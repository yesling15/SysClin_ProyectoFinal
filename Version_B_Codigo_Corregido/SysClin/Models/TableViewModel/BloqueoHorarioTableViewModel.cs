using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class BloqueoHorarioTableViewModel
    {
        public int ID_Bloqueo_Horario { get; set; }
        public int ID_Usuario { get; set; }
        public System.DateTime Fecha_Inicio { get; set; }
        public System.TimeSpan Hora_Inicio { get; set; }
        public System.DateTime Fecha_Fin { get; set; }
        public System.TimeSpan Hora_Fin { get; set; }
    }
}