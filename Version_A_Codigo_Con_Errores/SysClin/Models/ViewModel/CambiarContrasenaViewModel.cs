using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class CambiarContrasenaViewModel
    {
        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string ContrasenaActual { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 32 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$", ErrorMessage = "La contraseña debe incluir al menos una letra, un número y un carácter especial.")]
        [Display(Name = "Nueva contraseña")]
        public string NuevaContrasena { get; set; }

        [Required(ErrorMessage = "Debe confirmar su nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare("NuevaContrasena", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        [Display(Name = "Confirmar nueva contraseña")]
        public string ConfirmarNuevaContrasena { get; set; }
    }
}