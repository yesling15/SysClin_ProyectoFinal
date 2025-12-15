using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class RecuperacionContrasenaViewModel
    {
        public int ID_Recuperacion_Contrasena { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public int ID_Usuario { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de solicitud")]
        public DateTime Fecha_Solicitud { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de expiración")]
        public DateTime Fecha_Expiracion { get; set; }

        [Required]
        public bool Usado { get; set; }
    }
}