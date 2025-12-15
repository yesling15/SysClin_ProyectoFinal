using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class ConsultaDisponibilidadRequestViewModel
    {
        public int ID_Profesional { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
    }
}