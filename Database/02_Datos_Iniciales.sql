USE SysClin;
GO

/* ============================================
   Datos iniciales: TipoUsuario
   ============================================ */

INSERT INTO TipoUsuario (Nombre)
VALUES 
    (N'Paciente'),
    (N'Profesional');
GO

/* ============================================
   Datos iniciales: TipoIdentificacion
   ============================================ */

INSERT INTO TipoIdentificacion (Nombre)
VALUES
    (N'Cédula'),
    (N'DIMEX'),
    (N'Pasaporte');
GO

/* ============================================
   Datos iniciales: EstadoCita
   ============================================ */

INSERT INTO EstadoCita (Nombre)
VALUES
    (N'Programada'),
    (N'Cancelada'),
    (N'Reprogramada'),
    (N'Atendida'),
    (N'Inasistencia');
GO

/* ============================================
   Datos iniciales: Especialidad
   (puedes agregar más si lo necesitás)
   ============================================ */

INSERT INTO Especialidad (Nombre)
VALUES
    (N'Medicina General'),
    (N'Pediatría'),
    (N'Ginecología y Obstetricia'),
    (N'Cardiología'),
    (N'Dermatología'),
    (N'Psiquiatría'),
    (N'Neurología'),
    (N'Ortopedia y Traumatología'),
    (N'Oftalmología'),
    (N'Odontología'),
    (N'Nutrición');
GO
