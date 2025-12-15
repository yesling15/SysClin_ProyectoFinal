using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class RecuperacionContrasenaTableViewModel
    {
        public int ID_Recuperacion_Contrasena { get; set; }
        public int ID_Usuario { get; set; }
        public string Codigo { get; set; }
        public System.DateTime Fecha_Solicitud { get; set; }
        public System.DateTime Fecha_Expiracion { get; set; }
        public bool Usado { get; set; }
    }
}