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

        [Required]
        [Display(Name = "Usuario")]
        public int ID_Usuario { get; set; }

        [Required]
        [Display(Name = "Especialidad")]        
        public int? ID_Especialidad { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ0-9 ,\.]+$", ErrorMessage = "El campo {0} solo puede contener letras, números, espacios, comas y puntos")]
        [Display(Name = "Lugar de atención")]
        public string Lugar_Atencion { get; set; }

        [Display(Name = "Tiempo de anticipación")]
        [Range(1, int.MaxValue, ErrorMessage = "El tiempo de anticipación debe ser un número positivo")]
        public int? Tiempo_Anticipacion { get; set; }

        [StringLength(10, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Unidad de anticipación")]
        public string Unidad_Anticipacion { get; set; }
    }
}