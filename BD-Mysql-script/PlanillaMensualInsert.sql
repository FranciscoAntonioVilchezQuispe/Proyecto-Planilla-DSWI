USE BDPLANILLA;

-- SituacionTrabajador
INSERT INTO SituacionTrabajador (IdSituacion, Nombre, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'Activo', 1, '2024-10-14 19:18:13.577', NULL),
(2, 'Vacaciones', 1, '2024-10-14 19:18:22.700', NULL),
(3, 'Cesado', 1, '2024-10-14 19:18:34.153', NULL);

-- Cargos
INSERT INTO Cargos (IdCargo, Nombre, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'admnistrador', 1, '2024-09-28 22:00:58.777', '2024-10-03 10:09:42.893'),
(2, 'Ventas', 1, '2024-10-01 22:41:04.803', '2025-01-23 21:02:24.020'),
(1003, 'Choferes', 1, '2024-10-21 23:33:41.857', NULL),
(1004, 'informatico22', 1, '2024-10-22 10:28:40.447', '2024-10-22 10:29:33.080');

-- EstadosCiviles
INSERT INTO EstadosCiviles (IdEstadoCivil, Nombre, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'Soltero', 1, '2024-10-03 14:34:33.833', '2024-10-03 14:37:10.020'),
(2, 'Casado', 1, '2024-10-03 14:35:18.183', '2024-10-03 14:37:25.377');

-- Generos
INSERT INTO Generos (IdGenero, Nombre, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'Maculino', 1, '2024-10-03 14:31:36.500', '2024-10-03 14:33:35.873'),
(2, 'Femenino', 1, '2024-10-03 14:31:44.593', NULL);

-- TipoDocumentos
INSERT INTO TipoDocumentos (IdTipoDocumento, Nombre, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'Dni', 1, '2024-10-14 19:15:05.593', NULL),
(2, 'Cedula', 1, '2024-10-14 19:15:12.527', '2024-10-14 19:17:56.583'),
(3, 'Pasaporte', 1, '2024-10-14 19:15:18.720', NULL);

-- SistemaPensiones
INSERT INTO SistemaPensiones (IdSistemaPension, Nombre, Aporte, Comision, Prima, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 'ONP', 13.000, 0.000, 0.000, 1, '2024-10-14 20:19:42.630', NULL),
(2, 'PRIMA', 10.000, 1.470, 1.700, 1, '2024-10-14 20:21:10.997', NULL);

-- Trabajadores
INSERT INTO Trabajadores (IdTrabajador, IdTipoDocumento, Documento, Nombres, ApellidoPaterno, ApellidoMaterno, IdGenero, IdEstadoCivil, Direccion, Email, Hijos, IdCargo, FecNacimiento, FecIngreso, IdSituacion, IdSistemaPension, Foto, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 1, '44050058', 'Franciso', 'vilchez', 'quispe', 1, 1, '', '', 1, 1, '1986-12-19', '1986-12-19', 1, 1, NULL, 1, '2024-10-18 08:45:28.943', NULL),
(2, 1, '25325521', 'sdgfsdhg', 'sdgsdf', 'hiulujilhu', 1, 1, '', '', 4, 1, '2024-10-18', '2024-10-18', 3, 1, NULL, 1, '2024-10-18 08:53:39.547', '2024-10-18 08:56:05.327'),
(3, 1, '856213251', 'sbkfbnaskhf', 'sdghj', 'fdghdfr', 1, 1, '', '', 0, 1, '2024-10-21', '2024-10-21', 1, 2, NULL, 1, '2024-10-21 23:35:42.730', NULL);

-- PlanillaMensual
INSERT INTO PlanillaMensual (IdPlanillaMensual, Año, Mes, IdTrabajador, IdSituacion, IdCargo, Apellido, Nombre, IdSistemaPension, IdEstadoCivil, Hijos, FechaIngreso, SueldoBasico, PorcHoraExtra1, PorcHoraExtra2, PorcDescansoTrab, PorcFeriadoTrab, PorcAsigFamiliar, nHorasNormal, nHorasExtra1, nHorasExtra2, nDiasTrab, nDiasDescansos, nFeriadosTrab, nDescansosTrab, nDiasInasistencias, HaberBasico, ValesEmpleado, vHorasExtra1, vHorasExtra2, vAsigFamiliar, vDescansoTrab, vFeriadoTrab, BonificacionCargo, BonificacionMovilidad, CanastaNavidad, Escolaridad, DiaTrabajador, TotalIngreso, Renta5ta, DescuentoJud1, DescuentoJud2, DescuentoJud3, OtrosAdelantos, AdelantoCaja, AdelantoQuincena, AdelantoVac, AdelantoGratificacion, AdelantoLiquidacion, AdelantoCTS, PorcAporte, Aporte, PorcComision, Comision, PorcPrima, Prima, OTDSeg, OTDPacifico, IdBanco1, Prestamo1, Tardanza, TotalDescuento, PorcEsSalud, EsSalud, AccidenteTrab, Senati, SeguroVidaLey, TotalNeto, TotalNetoCad, TotalNetoBoleta, TotalNetoBoletaCad, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(6, 2024, 10, 1, 1, 1, 'vilchez quispe', 'Franciso', 1, 1, 1, '1986-12-19 00:00:00.000', 1024.00, 0.25, 0.35, 2.00, 2.00, 0.10, 216.00, 0.00, 0.00, 27, 4, 2, 0, 0, 1024.00, 100.00, 0.00, 0.00, 102.40, 0.00, 68.27, 100.00, 0.00, 0.00, 0.00, 0.00, 1394.67, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13.00, 181.31, 0.00, 0.00, 0.00, 0.00, NULL, NULL, NULL, NULL, NULL, 181.31, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1213.36, 'un mil doscientos trece con 36/100 SOLES', 1, '2024-10-22 10:34:48.963', NULL),
(7, 2024, 10, 2, 3, 1, 'sdgsdf hiulujilhu', 'sdgfsdhg', 1, 1, 4, '2024-10-18 00:00:00.000', 2500.00, 0.25, 0.35, 2.00, 2.00, 0.10, 200.00, 10.00, 15.00, 25, 5, 3, 0, 1, 2419.35, 145.16, 130.21, 210.94, 250.00, 0.00, 166.67, 145.16, 0.00, 0.00, 0.00, 0.00, 3467.49, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 13.00, 450.77, 0.00, 0.00, 0.00, 0.00, NULL, NULL, NULL, NULL, NULL, 450.77, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 3016.72, 'tres mil dieciseis con 72/100 SOLES', 1, '2024-10-22 10:34:48.963', NULL),
(8, 2024, 10, 3, 1, 1, 'sdghj fdghdfr', 'sbkfbnaskhf', 2, 1, 0, '2024-10-21 00:00:00.000', 1024.00, 0.25, 0.35, 2.00, 2.00, 0.10, 208.00, 2.00, 4.00, 26, 5, 1, 0, 0, 1024.00, 150.00, 10.67, 23.04, 0.00, 0.00, 68.27, 150.00, 0.00, 0.00, 0.00, 0.00, 1425.98, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 10.00, 142.60, 1.47, 20.96, 1.70, 24.24, NULL, NULL, NULL, NULL, NULL, 187.80, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1238.18, 'un mil doscientos treinta y ocho con 18/100 SOLES', 1, '2024-10-22 10:34:48.963', NULL);

-- IngresosTrabajadores
INSERT INTO IngresosTrabajadores (IdIngresoTrabajador, IdTrabajador, Remuneracion, Vale, BonifCargo, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 1, 1024.000, 100.000, 500.000, 1, '2024-10-18 15:22:16.293', '2024-10-22 10:31:43.487'),
(8, 2, 2500.000, 150.000, 1000.000, 1, '2024-10-19 11:18:29.127', NULL),
(9, 3, 1024.000, 150.000, 500.000, 1, '2024-10-21 23:36:22.097', NULL);

-- AsistenciasTrabajadores
INSERT INTO AsistenciasTrabajadores (IdAsistencia, IdTrabajador, Año, Mes, DiasLaborales, DiasDescanso, DiasInasistencia, DiasFeriados, HorasExtra25, HorasExtra35, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(15, 1, 2024, 10, 27, 4, 0, 2, 0.000, 0.000, 1, '2024-10-22 10:33:06.370', NULL),
(16, 2, 2024, 10, 25, 5, 1, 3, 10.000, 15.000, 1, '2024-10-22 10:33:06.370', NULL),
(17, 3, 2024, 10, 26, 5, 0, 1, 2.000, 4.000, 1, '2024-10-22 10:33:06.370', NULL);

-- Parametros
INSERT INTO Parametros (IdParametro, RemBasico, PorcAsigancionFamiliar, PorcExtra1, PorcExtra2, Activo, FecCreacion, FecUltimaModificacion) 
VALUES 
(1, 1024.000, 0.100, 0.250, 0.350, 1, '2024-10-18 14:48:49.553', NULL);

