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

        [Required]
        [Display(Name = "Tipo de usuario")]
        public int? ID_Tipo_Usuario { get; set; }

        [Required]
        [Display(Name = "Tipo de identificación")]
        public int? ID_Tipo_Identificacion { get; set; }

        [Required]
        [Display(Name = "Número de identificación")]
        public string Numero_Identificacion { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ \-]+$", ErrorMessage = "El campo {0} solo puede contener letras, espacios y guiones")]

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ \-]+$", ErrorMessage = "El campo {0} solo puede contener letras, espacios y guiones")]
        [Display(Name = "Primer apellido")]
        public string Primer_Apellido { get; set; }
                
        [StringLength(50, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñüÜ \-]+$", ErrorMessage = "El campo {0} solo puede contener letras, espacios y guiones")]
        [Display(Name = "Segundo apellido")]
        public string Segundo_Apellido { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [Display(Name = "Correo electrónico")]
        public string Correo_Electronico { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 32 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$", ErrorMessage = "La contraseña debe incluir al menos una letra, un número y un carácter especial.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de teléfono debe contener 8 dígitos numéricos")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? Fecha_Nacimiento { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden")]
        public string ConfirmarContrasena { get; set; }

    }
}