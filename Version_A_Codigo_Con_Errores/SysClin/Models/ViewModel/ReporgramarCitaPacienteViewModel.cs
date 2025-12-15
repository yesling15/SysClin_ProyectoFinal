using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class ReprogramarCitaPacienteViewModel
    {
        [Required]
        public int ID_Cita { get; set; }

        [Required]
        public int ID_Paciente { get; set; }

        [Required]
        public int ID_Profesional { get; set; }

        [Display(Name = "Profesional")]
        public string NombreProfesional { get; set; }

        [Display(Name = "Servicio")]
        public string Servicio { get; set; }

        [Display(Name = "Fecha actual")]
        public DateTime FechaActual { get; set; }

        [Display(Name = "Hora actual")]
        public string HoraActual { get; set; }

        [Required]
        [Display(Name = "Nueva fecha")]
        public DateTime NuevaFecha { get; set; }

        [Required]
        [Display(Name = "Nueva hora")]
        public string NuevaHora { get; set; }

        // Para el DropDownList de horarios disponibles
        //public IEnumerable<SelectListItem> HorariosDisponibles { get; set; }
    }
}