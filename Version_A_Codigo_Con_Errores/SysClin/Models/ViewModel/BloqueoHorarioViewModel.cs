using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class BloqueoHorarioViewModel
    {
        public int ID_Bloqueo_Horario { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public int ID_Usuario { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime Fecha_Inicio { get; set; }

        [Required]
        [Display(Name = "Hora de inicio")]
        public TimeSpan Hora_Inicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de fin")]
        public DateTime Fecha_Fin { get; set; }

        [Required]
        [Display(Name = "Hora de fin")]
        public TimeSpan Hora_Fin { get; set; }
    }
}