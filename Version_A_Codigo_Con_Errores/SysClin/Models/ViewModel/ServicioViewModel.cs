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

        
        [Display(Name = "Profesional")]
        public int ID_Usuario { get; set; }

        
        [Display(Name = "Nombre del servicio")]
        public string Nombre { get; set; }

        
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        
        [DataType(DataType.Currency)]
        
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
    }
}