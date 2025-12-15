-- Crear la base de datos
CREATE DATABASE SysClin;
GO

-- Usar la base de datos
USE SysClin;
GO

/* =========================================================
   1. Tablas de catálogos básicos (sin dependencias)
   ========================================================= */

-- Tabla TipoUsuario
CREATE TABLE TipoUsuario (
    ID_Tipo_Usuario INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL
);

-- Tabla TipoIdentificacion
CREATE TABLE TipoIdentificacion (
    ID_Tipo_Identificacion INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL
);

-- Tabla Especialidad
CREATE TABLE Especialidad (
    ID_Especialidad INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(50) NOT NULL
);

-- Tabla EstadoCita
CREATE TABLE EstadoCita (
    ID_Estado_Cita INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL
);

GO

/* =========================================================
   2. Tabla Usuario
   ========================================================= */

CREATE TABLE Usuario (
    ID_Usuario INT PRIMARY KEY IDENTITY,
    ID_Tipo_Usuario INT NOT NULL,
    ID_Tipo_Identificacion INT NOT NULL,

    Numero_Identificacion NVARCHAR(20) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Primer_Apellido NVARCHAR(50) NOT NULL,
    Segundo_Apellido NVARCHAR(50) NULL,
    Correo_Electronico NVARCHAR(100) NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    Telefono NVARCHAR(8) NOT NULL,
    Fecha_Nacimiento DATE NOT NULL,

    -- ÚNICOS (LOS ÚNICOS QUE VAN SEGÚN EL DOCUMENTO)
    CONSTRAINT UQ_Usuario_NumeroIdentificacion UNIQUE (Numero_Identificacion),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo_Electronico),

    CONSTRAINT FK_Usuario_TipoUsuario
        FOREIGN KEY (ID_Tipo_Usuario) REFERENCES TipoUsuario(ID_Tipo_Usuario),
    CONSTRAINT FK_Usuario_TipoIdentificacion
        FOREIGN KEY (ID_Tipo_Identificacion) REFERENCES TipoIdentificacion(ID_Tipo_Identificacion)
);

GO

/* =========================================================
   3. Perfil del profesional
   ========================================================= */

CREATE TABLE PerfilProfesional (
    ID_Perfil_Profesional INT PRIMARY KEY IDENTITY,
    ID_Usuario INT NOT NULL,
    ID_Especialidad INT NOT NULL,
    Lugar_Atencion NVARCHAR(100) NOT NULL,
    Tiempo_Anticipacion INT NULL,
    Unidad_Anticipacion NVARCHAR(10) NULL,

    CONSTRAINT FK_PerfilProfesional_Usuario
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
    CONSTRAINT FK_PerfilProfesional_Especialidad
        FOREIGN KEY (ID_Especialidad) REFERENCES Especialidad(ID_Especialidad)
);

GO

/* =========================================================
   4. Servicios del profesional
   ========================================================= */

CREATE TABLE Servicio (
    ID_Servicio INT PRIMARY KEY IDENTITY,
    ID_Usuario INT NOT NULL, -- Profesional
    Nombre NVARCHAR(60) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Precio DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_Servicio_Usuario
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

GO

/* =========================================================
   5. Horarios de atención
   ========================================================= */

CREATE TABLE HorarioAtencion (
    ID_Horario INT PRIMARY KEY IDENTITY,
    ID_Usuario INT NOT NULL,
    Dia_Semana NVARCHAR(15) NOT NULL,
    Hora_Inicio TIME NOT NULL,
    Hora_Fin TIME NOT NULL,
    Duracion_Minutos INT NOT NULL,

    CONSTRAINT FK_HorarioAtencion_Usuario
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

GO

/* =========================================================
   6. Bloqueos de agenda
   ========================================================= */

CREATE TABLE BloqueoHorario (
    ID_Bloqueo_Horario INT PRIMARY KEY IDENTITY,
    ID_Usuario INT NOT NULL,
    Fecha_Inicio DATE NOT NULL,
    Hora_Inicio TIME NOT NULL,
    Fecha_Fin DATE NOT NULL,
    Hora_Fin TIME NOT NULL,

    CONSTRAINT FK_BloqueoHorario_Usuario
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

GO

/* =========================================================
   7. Recuperación de contraseña
   ========================================================= */

CREATE TABLE RecuperacionContrasena (
    ID_Recuperacion_Contrasena INT PRIMARY KEY IDENTITY,
    ID_Usuario INT NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Fecha_Solicitud DATETIME2 NOT NULL,
    Fecha_Expiracion DATETIME2 NOT NULL,
    Usado BIT NOT NULL,

    CONSTRAINT FK_RecuperacionContrasena_Usuario
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario)
);

GO

/* =========================================================
   8. Citas
   ========================================================= */

CREATE TABLE Cita (
    ID_Cita INT PRIMARY KEY IDENTITY,
    ID_Paciente INT NOT NULL,
    ID_Profesional INT NOT NULL,
    ID_Servicio INT NOT NULL,
    ID_Estado_Cita INT NOT NULL,

    Fecha DATE NOT NULL,
    Hora TIME NOT NULL,
    Fecha_Creacion DATETIME2 NOT NULL,

    ID_Cita_Original INT NULL,

    CONSTRAINT FK_Cita_Paciente
        FOREIGN KEY (ID_Paciente) REFERENCES Usuario(ID_Usuario),
    CONSTRAINT FK_Cita_Profesional
        FOREIGN KEY (ID_Profesional) REFERENCES Usuario(ID_Usuario),
    CONSTRAINT FK_Cita_Servicio
        FOREIGN KEY (ID_Servicio) REFERENCES Servicio(ID_Servicio),
    CONSTRAINT FK_Cita_EstadoCita
        FOREIGN KEY (ID_Estado_Cita) REFERENCES EstadoCita(ID_Estado_Cita),
    CONSTRAINT FK_Cita_CitaOriginal
        FOREIGN KEY (ID_Cita_Original) REFERENCES Cita(ID_Cita)
);

GO
