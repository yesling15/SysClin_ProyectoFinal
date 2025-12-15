using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class ServicioViewModel
    {
        public int ID_Servicio { get; set; }

        [Required]
        [Display(Name = "Profesional")]
        public int ID_Usuario { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Nombre del servicio")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El {0} debe ser un valor positivo")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
    }
}