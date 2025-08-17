-- Crear base de datos BDPLANILLA en MySQL
CREATE DATABASE IF NOT EXISTS BDPLANILLA
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE BDPLANILLA;

-- Tabla TipoDocumentos
CREATE TABLE TipoDocumentos (
    IdTipoDocumento INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla Generos
CREATE TABLE Generos (
    IdGenero INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla EstadosCiviles
CREATE TABLE EstadosCiviles (
    IdEstadoCivil INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla Cargos
CREATE TABLE Cargos (
    IdCargo INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla SituacionTrabajador
CREATE TABLE SituacionTrabajador (
    IdSituacion INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla SistemaPensiones
CREATE TABLE SistemaPensiones (
    IdSistemaPension INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Aporte DECIMAL(18,3) NULL,
    Comision DECIMAL(18,3) NULL,
    Prima DECIMAL(18,3) NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla Trabajadores
CREATE TABLE Trabajadores (
    IdTrabajador INT AUTO_INCREMENT PRIMARY KEY,
    IdTipoDocumento INT NOT NULL,
    Documento VARCHAR(11) NOT NULL,
    Nombres VARCHAR(50) NOT NULL,
    ApellidoPaterno VARCHAR(50) NOT NULL,
    ApellidoMaterno VARCHAR(50) NOT NULL,
    IdGenero INT NOT NULL,
    IdEstadoCivil INT NOT NULL,
    Direccion VARCHAR(120) NOT NULL,
    Email VARCHAR(120) NULL,
    Hijos INT NOT NULL DEFAULT 0,
    IdCargo INT NOT NULL,
    FecNacimiento DATE NOT NULL,
    FecIngreso DATE NOT NULL,
    IdSituacion INT NOT NULL,
    IdSistemaPension INT NOT NULL,
    Foto LONGBLOB NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL,
    
    FOREIGN KEY (IdTipoDocumento) REFERENCES TipoDocumentos(IdTipoDocumento),
    FOREIGN KEY (IdGenero) REFERENCES Generos(IdGenero),
    FOREIGN KEY (IdEstadoCivil) REFERENCES EstadosCiviles(IdEstadoCivil),
    FOREIGN KEY (IdCargo) REFERENCES Cargos(IdCargo),
    FOREIGN KEY (IdSituacion) REFERENCES SituacionTrabajador(IdSituacion),
    FOREIGN KEY (IdSistemaPension) REFERENCES SistemaPensiones(IdSistemaPension)
);

-- Tabla AsistenciasTrabajadores
CREATE TABLE AsistenciasTrabajadores (
    IdAsistencia INT AUTO_INCREMENT PRIMARY KEY,
    IdTrabajador INT NULL,
    Año INT NULL,
    Mes INT NULL,
    DiasLaborales INT NULL,
    DiasDescanso INT NULL,
    DiasInasistencia INT NULL,
    DiasFeriados INT NULL,
    HorasExtra25 DECIMAL(18,3) NULL,
    HorasExtra35 DECIMAL(18,3) NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL,
    
    FOREIGN KEY (IdTrabajador) REFERENCES Trabajadores(IdTrabajador)
);

-- Tabla IngresosTrabajadores
CREATE TABLE IngresosTrabajadores (
    IdIngresoTrabajador INT AUTO_INCREMENT PRIMARY KEY,
    IdTrabajador INT NULL,
    Remuneracion DECIMAL(18,3) NULL,
    Vale DECIMAL(18,3) NULL,
    BonifCargo DECIMAL(18,3) NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL,
    
    FOREIGN KEY (IdTrabajador) REFERENCES Trabajadores(IdTrabajador)
);

-- Tabla Parametros
CREATE TABLE Parametros (
    IdParametro INT AUTO_INCREMENT PRIMARY KEY,
    RemBasico DECIMAL(18,3) NULL,
    PorcAsigancionFamiliar DECIMAL(18,3) NULL,
    PorcExtra1 DECIMAL(18,3) NULL,
    PorcExtra2 DECIMAL(18,3) NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL
);

-- Tabla PlanillaMensual
CREATE TABLE PlanillaMensual (
    IdPlanillaMensual BIGINT AUTO_INCREMENT PRIMARY KEY,
    Año INT NULL,
    Mes INT NULL,
    IdTrabajador INT NULL,
    IdSituacion INT NULL,
    IdCargo INT NULL,
    Apellido VARCHAR(40) NULL,
    Nombre VARCHAR(40) NULL,
    IdSistemaPension INT NULL,
    IdEstadoCivil INT NULL,
    Hijos SMALLINT NULL,
    FechaIngreso DATETIME NULL,
    SueldoBasico DECIMAL(18,2) NULL,
    PorcHoraExtra1 DECIMAL(18,2) NULL,
    PorcHoraExtra2 DECIMAL(18,2) NULL,
    PorcDescansoTrab DECIMAL(18,2) NULL,
    PorcFeriadoTrab DECIMAL(18,2) NULL,
    PorcAsigFamiliar DECIMAL(18,2) NULL,
    nHorasNormal DECIMAL(18,2) NULL,
    nHorasExtra1 DECIMAL(18,2) NULL,
    nHorasExtra2 DECIMAL(18,2) NULL,
    nDiasTrab SMALLINT NULL,
    nDiasDescansos SMALLINT NULL,
    nFeriadosTrab SMALLINT NULL,
    nDescansosTrab SMALLINT NULL,
    nDiasInasistencias SMALLINT NULL,
    HaberBasico DECIMAL(18,2) NULL,
    ValesEmpleado DECIMAL(18,2) NULL,
    vHorasExtra1 DECIMAL(18,2) NULL,
    vHorasExtra2 DECIMAL(18,2) NULL,
    vAsigFamiliar DECIMAL(18,2) NULL,
    vDescansoTrab DECIMAL(18,2) NULL,
    vFeriadoTrab DECIMAL(18,2) NULL,
    BonificacionCargo DECIMAL(18,2) NULL,
    BonificacionMovilidad DECIMAL(18,2) NULL,
    CanastaNavidad DECIMAL(18,2) NULL,
    Escolaridad DECIMAL(18,2) NULL,
    DiaTrabajador DECIMAL(18,2) NULL,
    TotalIngreso DECIMAL(18,2) NULL,
    Renta5ta DECIMAL(18,2) NULL,
    DescuentoJud1 DECIMAL(18,2) NULL,
    DescuentoJud2 DECIMAL(18,2) NULL,
    DescuentoJud3 DECIMAL(18,2) NULL,
    OtrosAdelantos DECIMAL(18,2) NULL,
    AdelantoCaja DECIMAL(18,2) NULL,
    AdelantoQuincena DECIMAL(18,2) NULL,
    AdelantoVac DECIMAL(18,2) NULL,
    AdelantoGratificacion DECIMAL(18,2) NULL,
    AdelantoLiquidacion DECIMAL(18,2) NULL,
    AdelantoCTS DECIMAL(18,2) NULL,
    PorcAporte DECIMAL(18,2) NULL,
    Aporte DECIMAL(18,2) NULL,
    PorcComision DECIMAL(18,2) NULL,
    Comision DECIMAL(18,2) NULL,
    PorcPrima DECIMAL(18,2) NULL,
    Prima DECIMAL(18,2) NULL,
    OTDSeg DECIMAL(18,2) NULL,
    OTDPacifico DECIMAL(18,2) NULL,
    IdBanco1 BIGINT NULL,
    Prestamo1 DECIMAL(18,2) NULL,
    Tardanza DECIMAL(18,2) NULL,
    TotalDescuento DECIMAL(18,2) NULL,
    PorcEsSalud DECIMAL(18,2) NULL,
    EsSalud DECIMAL(18,2) NULL,
    AccidenteTrab DECIMAL(18,2) NULL,
    Senati DECIMAL(18,2) NULL,
    SeguroVidaLey DECIMAL(18,2) NULL,
    TotalNeto DECIMAL(18,2) NULL,
    TotalNetoCad VARCHAR(180) NULL,
    TotalNetoBoleta DECIMAL(18,2) NULL,
    TotalNetoBoletaCad VARCHAR(180) NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,
    FecCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FecUltimaModificacion DATETIME NULL,
    
    FOREIGN KEY (IdTrabajador) REFERENCES Trabajadores(IdTrabajador),
    FOREIGN KEY (IdSituacion) REFERENCES SituacionTrabajador(IdSituacion),
    FOREIGN KEY (IdCargo) REFERENCES Cargos(IdCargo),
    FOREIGN KEY (IdSistemaPension) REFERENCES SistemaPensiones(IdSistemaPension),
    FOREIGN KEY (IdEstadoCivil) REFERENCES EstadosCiviles(IdEstadoCivil)
);

-- Crear índices para mejorar el rendimiento
CREATE INDEX idx_trabajadores_documento ON Trabajadores(Documento);
CREATE INDEX idx_trabajadores_nombres ON Trabajadores(Nombres, ApellidoPaterno, ApellidoMaterno);
CREATE INDEX idx_asistencias_trabajador ON AsistenciasTrabajadores(IdTrabajador, Año, Mes);
CREATE INDEX idx_planilla_trabajador ON PlanillaMensual(IdTrabajador, Año, Mes);
CREATE INDEX idx_planilla_periodo ON PlanillaMensual(Año, Mes);