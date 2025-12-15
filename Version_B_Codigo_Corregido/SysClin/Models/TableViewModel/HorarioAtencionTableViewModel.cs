using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class HorarioAtencionTableViewModel
    {
        public int ID_Horario { get; set; }
        public int ID_Usuario { get; set; }
        public string Dia_Semana { get; set; }
        public string Hora_Inicio { get; set; }
        public string Hora_Fin { get; set; }
        public int Duracion_Minutos { get; set; }
    }
}