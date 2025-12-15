using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class HorarioAtencionViewModel
    {
        public int ID_Horario { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public int ID_Usuario { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Día de la semana")]
        public string Dia_Semana { get; set; }

        [Required]
        [Display(Name = "Hora de inicio")]
        public TimeSpan Hora_Inicio { get; set; }

        [Required]
        [Display(Name = "Hora de fin")]
        public TimeSpan Hora_Fin { get; set; }

        [Required]
        [Display(Name = "Duración de espacios (minutos)")]
        public int Duracion_Minutos { get; set; }
    }
}