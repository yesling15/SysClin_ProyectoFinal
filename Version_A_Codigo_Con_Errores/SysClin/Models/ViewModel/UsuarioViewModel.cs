using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class UsuarioViewModel
    {
        public int ID_Usuario { get; set; }

        
        [Display(Name = "Tipo de usuario")]
        public int? ID_Tipo_Usuario { get; set; }

        
        [Display(Name = "Tipo de identificación")]
        public int? ID_Tipo_Identificacion { get; set; }

        
        [Display(Name = "Número de identificación")]
        public string Numero_Identificacion { get; set; }

        
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ \-]+$", ErrorMessage = "El campo {0} solo puede contener letras, espacios y guiones")]

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        
        [Display(Name = "Primer apellido")]
        public string Primer_Apellido { get; set; }
                
        
        [Display(Name = "Segundo apellido")]
        public string Segundo_Apellido { get; set; }

        
        [Display(Name = "Correo electrónico")]
        public string Correo_Electronico { get; set; }

        
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? Fecha_Nacimiento { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        
        public string ConfirmarContrasena { get; set; }

    }
}