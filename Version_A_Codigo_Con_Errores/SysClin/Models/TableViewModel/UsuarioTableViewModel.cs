using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class UsuarioTableViewModel
    {
        public int ID_Usuario { get; set; }
        public int ID_Tipo_Usuario { get; set; }
        public int ID_Tipo_Identificacion { get; set; }
        public string Numero_Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public string Correo_Electronico { get; set; }
        public string Contrasena { get; set; }
        public string Telefono { get; set; }
        public System.DateTime Fecha_Nacimiento { get; set; }
    }
}