using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysClin.Models.TableViewModel
{
    public class ServicioTableViewModel
    {
        public int ID_Servicio { get; set; }
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
    }
}