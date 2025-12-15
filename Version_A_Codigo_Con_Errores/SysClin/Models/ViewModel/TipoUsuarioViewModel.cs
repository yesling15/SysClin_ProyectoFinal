using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class TipoUsuarioViewModel
    {
        public int ID_Tipo_Usuario { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El campo {0} debe tener un máximo de {1} caracteres")]
        public string Nombre { get; set; }
    }
}