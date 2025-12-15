using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.ViewModel
{
    public class ProfesionalBusquedaViewModel
    {
        public int ID_Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Especialidad { get; set; }
        public string LugarAtencion { get; set; }
    }
}