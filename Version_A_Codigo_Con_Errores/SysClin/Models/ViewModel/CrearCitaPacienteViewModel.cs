using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SysClin.Models.ViewModel
{
    public class CrearCitaPacienteViewModel
    {
        
        public int? ID_Profesional { get; set; }

        
        public int? ID_Paciente { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        
        public TimeSpan Hora { get; set; }

        // ------- Servicio seleccionado -------
        
        [Display(Name = "Servicio")]
        public int? ID_Servicio { get; set; }

        // ------- Lista de servicios para el DropDown -------
        public List<SelectListItem> ServiciosDisponibles { get; set; }
            = new List<SelectListItem>();

        // ------- Datos de apoyo para mostrar -------
        public string NombreProfesional { get; set; }
        public string NombrePaciente { get; set; }
    }
}