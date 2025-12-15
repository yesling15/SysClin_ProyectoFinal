using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SysClin.Models
{
    public interface ISysClinContext
    {
        DbSet<Usuario> Usuario { get; }
        DbSet<TipoUsuario> TipoUsuario { get; }
        DbSet<TipoIdentificacion> TipoIdentificacion { get; }
        DbSet<PerfilProfesional> PerfilProfesional { get; }
        DbSet<Especialidad> Especialidad { get; }
        DbSet<Servicio> Servicio { get; }
        DbSet<HorarioAtencion> HorarioAtencion { get; }
        DbSet<BloqueoHorario> BloqueoHorario { get; }
        DbSet<Cita> Cita { get; }
        DbSet<EstadoCita> EstadoCita { get; }
        int SaveChanges();
    }
}