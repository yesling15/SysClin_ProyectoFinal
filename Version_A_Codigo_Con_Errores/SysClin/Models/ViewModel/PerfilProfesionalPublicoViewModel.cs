using System.Collections.Generic;

namespace SysClin.Models.ViewModel
{
    public class PerfilProfesionalPublicoViewModel
    {
        public int ID_Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Especialidad { get; set; }
        public string LugarAtencion { get; set; }

        // Servicios que ofrece el profesional
        public List<ServicioViewModel> Servicios { get; set; }
    }
}