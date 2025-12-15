using SysClin.Models;
using System.Collections.Generic;
using System.Data.Entity;

public class FakeSysClinContext : ISysClinContext
{
    public FakeSysClinContext()
    {
        UsuariosData = new List<Usuario>();
        Usuario = new FakeDbSet<Usuario>(UsuariosData);

        TipoUsuario = new FakeDbSet<TipoUsuario>(new List<TipoUsuario>());
        TipoIdentificacion = new FakeDbSet<TipoIdentificacion>(new List<TipoIdentificacion>());
        PerfilProfesional = new FakeDbSet<PerfilProfesional>(new List<PerfilProfesional>());
        Especialidad = new FakeDbSet<Especialidad>(new List<Especialidad>());
        Servicio = new FakeDbSet<Servicio>(new List<Servicio>());
        HorarioAtencion = new FakeDbSet<HorarioAtencion>(new List<HorarioAtencion>());
        BloqueoHorario = new FakeDbSet<BloqueoHorario>(new List<BloqueoHorario>());
        Cita = new FakeDbSet<Cita>(new List<Cita>());
        EstadoCita = new FakeDbSet<EstadoCita>(new List<EstadoCita>());
    }

    public List<Usuario> UsuariosData { get; set; }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<TipoUsuario> TipoUsuario { get; set; }
    public DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }
    public DbSet<PerfilProfesional> PerfilProfesional { get; set; }
    public DbSet<Especialidad> Especialidad { get; set; }
    public DbSet<Servicio> Servicio { get; set; }
    public DbSet<HorarioAtencion> HorarioAtencion { get; set; }
    public DbSet<BloqueoHorario> BloqueoHorario { get; set; }
    public DbSet<Cita> Cita { get; set; }
    public DbSet<EstadoCita> EstadoCita { get; set; }

    public int SaveChanges()
    {
        return 1;
    }

    public void Dispose() { }
}