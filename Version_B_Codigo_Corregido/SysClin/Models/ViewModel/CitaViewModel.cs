using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class CitaViewModel
    {
        public int ID_Cita { get; set; }

        [Required]
        [Display(Name = "Paciente")]
        public int ID_Paciente { get; set; }

        [Required]
        [Display(Name = "Profesional")]
        public int ID_Profesional { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public int ID_Servicio { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public int ID_Estado_Cita { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Hora")]
        public TimeSpan Hora { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de creación")]
        public DateTime Fecha_Creacion { get; set; }

        [Display(Name = "Cita original")]
        public int? ID_Cita_Original { get; set; }
    }
}