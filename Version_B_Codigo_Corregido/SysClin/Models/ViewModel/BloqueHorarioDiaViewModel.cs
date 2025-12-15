using System;
using System.ComponentModel.DataAnnotations;

namespace SysClin.Models.ViewModel
{
    public class BloqueHorarioDiaViewModel
    {        
        public TimeSpan HoraInicio { get; set; }        
        public TimeSpan HoraFin { get; set; }                
        public int DuracionMinutos { get; set; }
    }
}
