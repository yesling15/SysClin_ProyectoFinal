using System.Collections.Generic;
using System.Web.Mvc;

namespace SysClin.Models.ViewModel
{
    public class FiltroBusquedaProfesionalViewModel
    {
        public string Nombre { get; set; }
        public int? ID_Especialidad { get; set; }
        public string Lugar { get; set; }

        // Lista para el DropDownList
        public List<SelectListItem> Especialidades { get; set; }

        // Resultados de la búsqueda
        public List<ProfesionalBusquedaViewModel> Resultados { get; set; }

        public FiltroBusquedaProfesionalViewModel()
        {
            Especialidades = new List<SelectListItem>();
            Resultados = new List<ProfesionalBusquedaViewModel>();
        }
    }
}