using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SysClin.Models.ViewModel
{
    public class DisponibilidadViewModel
    {
        [Display(Name = "ID del profesional")]
        public int ID_Profesional { get; set; }

        [Display(Name = "Profesional")]
        public string NombreProfesional { get; set; }

        [Display(Name = "Fecha consultada")]
        public DateTime Fecha { get; set; }

        // Bloques laborales del día según HorarioAtencion
        public List<BloqueHorarioDiaViewModel> BloquesDelDia { get; set; }
            = new List<BloqueHorarioDiaViewModel>();

        // Horarios con citas ya programadas
        public List<TimeSpan> HorasOcupadas { get; set; } = new List<TimeSpan>();

        // Bloqueos manuales del profesional
        public List<TimeSpan> HorasBloqueadas { get; set; } = new List<TimeSpan>();

        // ---- Horarios finales disponibles para reservar ----
        public List<TimeSpan> HorasDisponibles
        {
            get
            {
                var disponibles = new List<TimeSpan>();

                foreach (var bloque in BloquesDelDia)
                {
                    var h = bloque.HoraInicio;

                    while (h < bloque.HoraFin)
                    {
                        // Regla 1: No mostrar horas ya pasadas si la fecha es hoy
                        if (Fecha.Date == DateTime.Today && h <= DateTime.Now.TimeOfDay)
                        {
                            h = h.Add(TimeSpan.FromMinutes(bloque.DuracionMinutos));
                            continue;
                        }

                        // Regla 2: No mostrar bloqueos
                        if (HorasBloqueadas.Contains(h))
                        {
                            h = h.Add(TimeSpan.FromMinutes(bloque.DuracionMinutos));
                            continue;
                        }

                        // Regla 3: No mostrar citas ocupadas
                        if (HorasOcupadas.Contains(h))
                        {
                            h = h.Add(TimeSpan.FromMinutes(bloque.DuracionMinutos));
                            continue;
                        }

                        // Slot disponible
                        disponibles.Add(h);

                        h = h.Add(TimeSpan.FromMinutes(bloque.DuracionMinutos));
                    }
                }

                return disponibles;
            }
        }
    }
}
