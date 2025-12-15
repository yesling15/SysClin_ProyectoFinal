using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class PerfilProfesionalViewModel
    {
        public int ID_Perfil_Profesional { get; set; }

        
        [Display(Name = "Usuario")]
        public int ID_Usuario { get; set; }

        
        [Display(Name = "Especialidad")]        
        public int? ID_Especialidad { get; set; }

        
        [Display(Name = "Lugar de atención")]
        public string Lugar_Atencion { get; set; }

        [Display(Name = "Tiempo de anticipación")]
        
        public int? Tiempo_Anticipacion { get; set; }

        
        [Display(Name = "Unidad de anticipación")]
        public string Unidad_Anticipacion { get; set; }
    }
}