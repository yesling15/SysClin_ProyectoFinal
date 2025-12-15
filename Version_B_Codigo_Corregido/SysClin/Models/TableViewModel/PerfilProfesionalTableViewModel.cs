using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class PerfilProfesionalTableViewModel
    {
        public int ID_Perfil_Profesional { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Especialidad { get; set; }
        public string Lugar_Atencion { get; set; }
        public int? Tiempo_Anticipacion { get; set; }
        public string Unidad_Anticipacion { get; set; }
    }
}