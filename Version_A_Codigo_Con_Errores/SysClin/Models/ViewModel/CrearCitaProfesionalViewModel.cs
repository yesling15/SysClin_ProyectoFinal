using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    namespace SysClin.Models.ViewModel
    {
        public class CrearCitaProfesionalViewModel
        {
            public int ID_Profesional { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un paciente.")]
            [Display(Name = "Paciente")]
            public int ID_Paciente { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un servicio.")]
            [Display(Name = "Servicio")]
            public int ID_Servicio { get; set; }

            [Required(ErrorMessage = "Debe seleccionar una fecha.")]
            [Display(Name = "Fecha de la cita")]
            [DataType(DataType.Date)]
            public DateTime Fecha_Cita { get; set; }

            [Required(ErrorMessage = "Debe seleccionar una hora disponible.")]
            [Display(Name = "Hora de la cita")]
            public string Hora_Cita { get; set; }

            [Required(ErrorMessage = "Debe ingresar un motivo.")]
            [Display(Name = "Motivo de la consulta")]
            [StringLength(300, ErrorMessage = "El motivo no puede superar los 300 caracteres.")]
            public string Motivo { get; set; }
        }
    }
}