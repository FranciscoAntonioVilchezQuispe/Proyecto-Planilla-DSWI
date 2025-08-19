using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils.Request;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class PlanillaMensualLog
    {
        public List<PlanillaMensual> CalcularPlanillaByPeriodo(int año, int mes)
        {
 
            List<PlanillaMensual> arr = new List<PlanillaMensual>();
            var parametro = new ParametrosLog().BusquedaOne() ?? throw new Exception("No se encontraron parámetros del sistema"); ;
            var Arrtrabajador = new TrabajadorLog().Busqueda(new BuquedaTrabajador { Busqueda = "", Estado = _Estado.Activo }) ?? throw new Exception("Error al obtener trabajadores");
            var ArrIngresos = new IngresosTrabajadoresLog().Busqueda();
            var ArrAsistencia = new AsistenciaTrabajadorLog().BuscarAsistenciaByPeriodo(año, mes);
            var ArrSistemaPension = new SistemaPensionLog().Busqueda();
            int diasMes = DateTime.DaysInMonth(año, mes);
            
            foreach (var ItemTrabajador in Arrtrabajador)
            {
                PlanillaMensual obj = new PlanillaMensual();
                var itemAsistencia = ArrAsistencia.FirstOrDefault(r => r.IdTrabajador == ItemTrabajador.IdTrabajador);
                var itemIngreso = ArrIngresos.FirstOrDefault(r => r.IdTrabajador == ItemTrabajador.IdTrabajador);
                int diascalculo = (int)itemAsistencia.DiasLaborales + (int)itemAsistencia.DiasDescanso;

                obj.Año = año;
                obj.Mes = mes;
                obj.IdTrabajador = ItemTrabajador.IdTrabajador;
                obj.IdSituacion = ItemTrabajador.IdSituacion;
                obj.IdCargo = ItemTrabajador.IdCargo;
                obj.Apellido = $"{ItemTrabajador.ApellidoPaterno} {ItemTrabajador.ApellidoMaterno}";
                obj.Nombre = ItemTrabajador.Nombres;
                obj.IdSistemaPension = ItemTrabajador.IdSistemaPension;
                obj.IdEstadoCivil = ItemTrabajador.IdEstadoCivil;
                obj.Hijos = (short)ItemTrabajador.Hijos;
                obj.FechaIngreso = ItemTrabajador.FecIngreso;
                obj.SueldoBasico = itemIngreso?.Remuneracion;
                obj.PorcHoraExtra1 = parametro.PorcExtra1;
                obj.PorcHoraExtra2 = parametro.PorcExtra2;
                obj.PorcDescansoTrab = 2;
                obj.PorcFeriadoTrab = 2;
                obj.PorcAsigFamiliar = parametro.PorcAsigancionFamiliar;
                obj.nHorasNormal = itemAsistencia.DiasLaborales * 8;
                obj.nHorasExtra1 = itemAsistencia.HorasExtra25;
                obj.nHorasExtra2 = itemAsistencia.HorasExtra35;
                obj.nDiasTrab = (short)itemAsistencia.DiasLaborales;
                obj.nDiasDescansos = (short)itemAsistencia.DiasDescanso;
                obj.nFeriadosTrab = (short)itemAsistencia.DiasFeriados;
                obj.nDescansosTrab = 0;
                obj.nDiasInasistencias = (short)itemAsistencia.DiasInasistencia;
                obj.HaberBasico = Math.Round(((decimal)itemIngreso?.Remuneracion / diasMes) * diascalculo, 2, MidpointRounding.AwayFromZero);
                obj.ValesEmpleado = Math.Round(((decimal)itemIngreso?.Vale / diasMes) * diascalculo, 2, MidpointRounding.AwayFromZero);
                decimal valorhora = (decimal)obj.SueldoBasico / 30 / 8;
                obj.vHorasExtra1 = Math.Round((valorhora * (1 + (decimal)obj.PorcHoraExtra1)) * (decimal)obj.nHorasExtra1, 2, MidpointRounding.AwayFromZero);
                obj.vHorasExtra2 = Math.Round((valorhora * (1 + (decimal)obj.PorcHoraExtra2)) * (decimal)obj.nHorasExtra2, 2, MidpointRounding.AwayFromZero);
                obj.vAsigFamiliar = ItemTrabajador.Hijos == 0 ? 0 : Math.Round((decimal)obj.SueldoBasico * (decimal)obj.PorcAsigFamiliar, 2, MidpointRounding.AwayFromZero);
                obj.vDescansoTrab = 0;
                obj.vFeriadoTrab = Math.Round((valorhora * 8) * (decimal)obj.PorcFeriadoTrab, 2, MidpointRounding.AwayFromZero);
                obj.BonificacionCargo = Math.Round(((decimal)itemIngreso?.Vale / diasMes) * diascalculo, 2, MidpointRounding.AwayFromZero);
                obj.BonificacionMovilidad = 0;
                obj.CanastaNavidad = 0;
                obj.Escolaridad = 0;
                obj.DiaTrabajador = 0;
                obj.TotalIngreso = Math.Round((decimal)obj.HaberBasico + (decimal)obj.ValesEmpleado + (decimal)obj.vHorasExtra1 + (decimal)obj.vHorasExtra2 + (decimal)obj.vAsigFamiliar +
                                   (decimal)obj.vDescansoTrab + (decimal)obj.vFeriadoTrab + (decimal)obj.BonificacionCargo + (decimal)obj.BonificacionMovilidad + (decimal)obj.CanastaNavidad +
                                   (decimal)obj.Escolaridad + (decimal)obj.DiaTrabajador, 2, MidpointRounding.AwayFromZero);
                var itemsistemapension = ArrSistemaPension.FirstOrDefault(r => r.IdSistemaPension == ItemTrabajador.IdSistemaPension);
                obj.PorcAporte = itemsistemapension.Aporte;
                obj.Aporte = Math.Round((decimal)obj.TotalIngreso * ((decimal)obj.PorcAporte / 100), 2, MidpointRounding.AwayFromZero);
                obj.PorcComision = itemsistemapension.Comision;
                obj.Comision = Math.Round((decimal)obj.TotalIngreso * ((decimal)obj.PorcComision / 100), 2, MidpointRounding.AwayFromZero);
                obj.PorcPrima = itemsistemapension.Prima;
                obj.Prima = Math.Round((decimal)obj.TotalIngreso * ((decimal)obj.PorcPrima / 100), 2, MidpointRounding.AwayFromZero);
                obj.TotalDescuento = Math.Round((decimal)obj.Aporte + (decimal)obj.Comision + (decimal)obj.Prima, 2, MidpointRounding.AwayFromZero);
                obj.TotalNetoBoleta = Math.Round((decimal)obj.TotalIngreso - (decimal)obj.TotalDescuento, 2, MidpointRounding.AwayFromZero);
                obj.TotalNetoBoletaCad = Proyecto_Planilla_Utils.NumberToLetters.ToCardinal((decimal)obj.TotalNetoBoleta) + " SOLES";
                arr.Add(obj);
            }
            return arr;
            
        }

        
        public bool InsertarLista(List<PlanillaMensual> arr)

        {
            Console.WriteLine($"=== INICIO InsertarLista ===");
            Console.WriteLine($"Número de elementos recibidos: {arr?.Count ?? 0}");

            if (arr == null || arr.Count == 0)
            {
                Console.WriteLine("ERROR: Lista nula o vacía");
                return false;
            }
            try
            {
                // Validación inicial mejorada
                if (arr == null || arr.Count == 0)
                {
                    Console.WriteLine("Array de planillas es nulo o vacío");
                    return false;
                }

                // Log del primer elemento para diagnóstico

                var primerElemento = arr.FirstOrDefault();
                Console.WriteLine("=== DATOS DEL PRIMER ELEMENTO ===");
                Console.WriteLine($"Año: {primerElemento?.Año}");
                Console.WriteLine($"Mes: {primerElemento?.Mes}");
                Console.WriteLine($"Trabajador: {primerElemento?.Nombre} {primerElemento?.Apellido}");
                Console.WriteLine($"Sueldo Básico: {primerElemento?.SueldoBasico}");
                Console.WriteLine("================================");


                // Validar período coherente
                if (arr.Any(x => x.Año <= 0 || x.Mes <= 0 || x.Mes > 12))
                {
                    Console.WriteLine("Período inválido detectado en algunos registros");
                    throw new ArgumentException("Período inválido en los registros");
                }

                // Configurar auditoría
                Console.WriteLine("Configurando campos de auditoría...");
                foreach (var item in arr)
                {
                    new AuditoriaLog().SetAuditFieldsForInsert(item);
                    Console.WriteLine($"Auditoría configurada para: {item.Nombre} {item.Apellido}");
                }

                // Eliminar registros existentes con mejor logging
                Console.WriteLine($"Eliminando registros existentes para año: {arr[0].Año}, mes: {arr[0].Mes}");
                string deleteSql = "DELETE FROM PlanillaMensual WHERE Año = @Año AND Mes = @Mes";
                var deleteParameters = new MySqlParameter[]
                {
            new MySqlParameter("@Año", arr[0].Año),
            new MySqlParameter("@Mes", arr[0].Mes)
                };

                bool success = ADOConnection.ExecuteNonQuery(deleteSql, deleteParameters);
                Console.WriteLine($"Operación de eliminación {(success ? "exitosa" : "fallida")}");

                // Insertar nuevos registros
                string insertSql = @"
                                    INSERT INTO PlanillaMensual
                                    (
                                        `Año`, `Mes`, IdTrabajador, IdSituacion, IdCargo, Apellido, Nombre,
                                        IdSistemaPension, IdEstadoCivil, Hijos, FechaIngreso, SueldoBasico,
                                        PorcHoraExtra1, PorcHoraExtra2, PorcDescansoTrab, PorcFeriadoTrab,
                                        PorcAsigFamiliar, nHorasNormal, nHorasExtra1, nHorasExtra2, nDiasTrab,
                                        nDiasDescansos, nFeriadosTrab, nDescansosTrab, nDiasInasistencias,
                                        HaberBasico, ValesEmpleado, vHorasExtra1, vHorasExtra2, vAsigFamiliar,
                                        vDescansoTrab, vFeriadoTrab, BonificacionCargo, BonificacionMovilidad,
                                        CanastaNavidad, Escolaridad, DiaTrabajador, TotalIngreso,
                                        Renta5ta, DescuentoJud1, DescuentoJud2, DescuentoJud3, OtrosAdelantos,
                                        AdelantoCaja, AdelantoQuincena, AdelantoVac, AdelantoGratificacion,
                                        AdelantoLiquidacion, AdelantoCTS,
                                        PorcAporte, Aporte, PorcComision, Comision, PorcPrima, Prima,
                                        OTDSeg, OTDPacifico, IdBanco1, Prestamo1, Tardanza,
                                        TotalDescuento, PorcEsSalud, EsSalud, AccidenteTrab, Senati, SeguroVidaLey,
                                        TotalNeto, TotalNetoCad, TotalNetoBoleta, TotalNetoBoletaCad,
                                        Activo, FecCreacion, FecUltimaModificacion
                                    )
                                    VALUES
                                    (
                                        @Año, @Mes, @IdTrabajador, @IdSituacion, @IdCargo, @Apellido, @Nombre,
                                        @IdSistemaPension, @IdEstadoCivil, @Hijos, @FechaIngreso, @SueldoBasico,
                                        @PorcHoraExtra1, @PorcHoraExtra2, @PorcDescansoTrab, @PorcFeriadoTrab,
                                        @PorcAsigFamiliar, @nHorasNormal, @nHorasExtra1, @nHorasExtra2, @nDiasTrab,
                                        @nDiasDescansos, @nFeriadosTrab, @nDescansosTrab, @nDiasInasistencias,
                                        @HaberBasico, @ValesEmpleado, @vHorasExtra1, @vHorasExtra2, @vAsigFamiliar,
                                        @vDescansoTrab, @vFeriadoTrab, @BonificacionCargo, @BonificacionMovilidad,
                                        @CanastaNavidad, @Escolaridad, @DiaTrabajador, @TotalIngreso,
                                        @Renta5ta, @DescuentoJud1, @DescuentoJud2, @DescuentoJud3, @OtrosAdelantos,
                                        @AdelantoCaja, @AdelantoQuincena, @AdelantoVac, @AdelantoGratificacion,
                                        @AdelantoLiquidacion, @AdelantoCTS,
                                        @PorcAporte, @Aporte, @PorcComision, @Comision, @PorcPrima, @Prima,
                                        @OTDSeg, @OTDPacifico, @IdBanco1, @Prestamo1, @Tardanza,
                                        @TotalDescuento, @PorcEsSalud, @EsSalud, @AccidenteTrab, @Senati, @SeguroVidaLey,
                                        @TotalNeto, @TotalNetoCad, @TotalNetoBoleta, @TotalNetoBoletaCad,
                                        @Activo, @FecCreacion, @FecUltimaModificacion
                                    )";

                using (var connection = new MySqlConnection(ADOConnection.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var planilla in arr)
                            {
                                Console.WriteLine($"Procesando: {planilla.Nombre} {planilla.Apellido}");

                                // Validación de datos críticos
                                if (planilla.SueldoBasico == null || planilla.SueldoBasico <= 0)
                                {
                                    Console.WriteLine($"Advertencia: Sueldo básico inválido para {planilla.Nombre} {planilla.Apellido}");
                                }

                                var parameters = new MySqlParameter[]
                                {

                                new MySqlParameter("@Año", planilla.Año ?? (object)DBNull.Value),
                                new MySqlParameter("@Mes", planilla.Mes ?? (object)DBNull.Value),
                                new MySqlParameter("@IdTrabajador", planilla.IdTrabajador ?? (object)DBNull.Value),
                                new MySqlParameter("@IdSituacion", planilla.IdSituacion ?? (object)DBNull.Value),
                                new MySqlParameter("@IdCargo", planilla.IdCargo ?? (object)DBNull.Value),
                                new MySqlParameter("@Apellido", planilla.Apellido ?? (object)DBNull.Value),
                                new MySqlParameter("@Nombre", planilla.Nombre ?? (object)DBNull.Value),
                                new MySqlParameter("@IdSistemaPension", planilla.IdSistemaPension ?? (object)DBNull.Value),
                                new MySqlParameter("@IdEstadoCivil", planilla.IdEstadoCivil ?? (object)DBNull.Value),
                                new MySqlParameter("@Hijos", planilla.Hijos ?? (object)DBNull.Value),
                                new MySqlParameter("@FechaIngreso", planilla.FechaIngreso ?? (object)DBNull.Value),
                                new MySqlParameter("@SueldoBasico", planilla.SueldoBasico ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcHoraExtra1", planilla.PorcHoraExtra1 ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcHoraExtra2", planilla.PorcHoraExtra2 ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcDescansoTrab", planilla.PorcDescansoTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcFeriadoTrab", planilla.PorcFeriadoTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcAsigFamiliar", planilla.PorcAsigFamiliar ?? (object)DBNull.Value),
                                new MySqlParameter("@nHorasNormal", planilla.nHorasNormal ?? (object)DBNull.Value),
                                new MySqlParameter("@nHorasExtra1", planilla.nHorasExtra1 ?? (object)DBNull.Value),
                                new MySqlParameter("@nHorasExtra2", planilla.nHorasExtra2 ?? (object)DBNull.Value),
                                new MySqlParameter("@nDiasTrab", planilla.nDiasTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@nDiasDescansos", planilla.nDiasDescansos ?? (object)DBNull.Value),
                                new MySqlParameter("@nFeriadosTrab", planilla.nFeriadosTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@nDescansosTrab", planilla.nDescansosTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@nDiasInasistencias", planilla.nDiasInasistencias ?? (object)DBNull.Value),
                                new MySqlParameter("@HaberBasico", planilla.HaberBasico ?? (object)DBNull.Value),
                                new MySqlParameter("@ValesEmpleado", planilla.ValesEmpleado ?? (object)DBNull.Value),
                                new MySqlParameter("@vHorasExtra1", planilla.vHorasExtra1 ?? (object)DBNull.Value),
                                new MySqlParameter("@vHorasExtra2", planilla.vHorasExtra2 ?? (object)DBNull.Value),
                                new MySqlParameter("@vAsigFamiliar", planilla.vAsigFamiliar ?? (object)DBNull.Value),
                                new MySqlParameter("@vDescansoTrab", planilla.vDescansoTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@vFeriadoTrab", planilla.vFeriadoTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@BonificacionCargo", planilla.BonificacionCargo ?? (object)DBNull.Value),
                                new MySqlParameter("@BonificacionMovilidad", planilla.BonificacionMovilidad ?? (object)DBNull.Value),
                                new MySqlParameter("@CanastaNavidad", planilla.CanastaNavidad ?? (object)DBNull.Value),
                                new MySqlParameter("@Escolaridad", planilla.Escolaridad ?? (object)DBNull.Value),
                                new MySqlParameter("@DiaTrabajador", planilla.DiaTrabajador ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalIngreso", planilla.TotalIngreso ?? (object)DBNull.Value),
                                new MySqlParameter("@Renta5ta", planilla.Renta5ta ?? (object)DBNull.Value),
                                new MySqlParameter("@DescuentoJud1", planilla.DescuentoJud1 ?? (object)DBNull.Value),
                                new MySqlParameter("@DescuentoJud2", planilla.DescuentoJud2 ?? (object)DBNull.Value),
                                new MySqlParameter("@DescuentoJud3", planilla.DescuentoJud3 ?? (object)DBNull.Value),
                                new MySqlParameter("@OtrosAdelantos", planilla.OtrosAdelantos ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoCaja", planilla.AdelantoCaja ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoQuincena", planilla.AdelantoQuincena ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoVac", planilla.AdelantoVac ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoGratificacion", planilla.AdelantoGratificacion ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoLiquidacion", planilla.AdelantoLiquidacion ?? (object)DBNull.Value),
                                new MySqlParameter("@AdelantoCTS", planilla.AdelantoCTS ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcAporte", planilla.PorcAporte ?? (object)DBNull.Value),
                                new MySqlParameter("@Aporte", planilla.Aporte ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcComision", planilla.PorcComision ?? (object)DBNull.Value),
                                new MySqlParameter("@Comision", planilla.Comision ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcPrima", planilla.PorcPrima ?? (object)DBNull.Value),
                                new MySqlParameter("@Prima", planilla.Prima ?? (object)DBNull.Value),
                                new MySqlParameter("@OTDSeg", planilla.OTDSeg ?? (object)DBNull.Value),
                                new MySqlParameter("@OTDPacifico", planilla.OTDPacifico ?? (object)DBNull.Value),
                                new MySqlParameter("@IdBanco1", planilla.IdBanco1 ?? (object)DBNull.Value),
                                new MySqlParameter("@Prestamo1", planilla.Prestamo1 ?? (object)DBNull.Value),
                                new MySqlParameter("@Tardanza", planilla.Tardanza ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalDescuento", planilla.TotalDescuento ?? (object)DBNull.Value),
                                new MySqlParameter("@PorcEsSalud", planilla.PorcEsSalud ?? (object)DBNull.Value),
                                new MySqlParameter("@EsSalud", planilla.EsSalud ?? (object)DBNull.Value),
                                new MySqlParameter("@AccidenteTrab", planilla.AccidenteTrab ?? (object)DBNull.Value),
                                new MySqlParameter("@Senati", planilla.Senati ?? (object)DBNull.Value),
                                new MySqlParameter("@SeguroVidaLey", planilla.SeguroVidaLey ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalNeto", planilla.TotalNeto ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalNetoCad", planilla.TotalNetoCad ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalNetoBoleta", planilla.TotalNetoBoleta ?? (object)DBNull.Value),
                                new MySqlParameter("@TotalNetoBoletaCad", planilla.TotalNetoBoletaCad ?? (object)DBNull.Value),
                                new MySqlParameter("@Activo", planilla.Activo),
                                new MySqlParameter("@FecCreacion", planilla.FecCreacion ?? (object)DBNull.Value),
                                new MySqlParameter("@FecUltimaModificacion", planilla.FecUltimaModificacion ?? (object)DBNull.Value)
                                };
                                
               

                                using (var command = new MySqlCommand(insertSql, connection, transaction))
                                {
                                    command.Parameters.AddRange(parameters);
                                    int affectedRows = command.ExecuteNonQuery();
                                    Console.WriteLine($"Insertado: {planilla.Nombre} {planilla.Apellido} - Filas afectadas: {affectedRows}");
                                }
                            }

                            transaction.Commit();
                            Console.WriteLine("Transacción completada con éxito");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR durante la inserción: {ex.Message}");
                            Console.WriteLine($"StackTrace: {ex.StackTrace}");
                            transaction.Rollback();
                            throw new Exception("Error al guardar la planilla. Detalles: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL en InsertarLista: {ex.Message}");
                throw;
            }
        }

        


        public List<PlanillaMensual> Lista(int año, int mes)
        {
            string cadena = $@"SELECT * FROM PlanillaMensual WHERE Año = @Año AND Mes = @Mes";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Año", año),
                new MySqlParameter("@Mes", mes)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<PlanillaMensual>(dataTable);
        }

        public string BuscarBoleta(BusquedaBoleta objboleta)
        {
            string cadena = $@"SELECT * FROM PlanillaMensual 
                              WHERE Año = @Año AND Mes = @Mes AND IdTrabajador IN 
                              (SELECT IdTrabajador FROM Trabajadores WHERE Documento = @Documento)";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Año", objboleta.Año),
                new MySqlParameter("@Mes", objboleta.Mes),
                new MySqlParameter("@Documento", objboleta.Documento)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            if (dataTable.Rows.Count == 0)
                return "No se encontró la boleta";

            var obj = ADOConnection.MapDataRowToObject<PlanillaMensual>(dataTable.Rows[0]);
            var objtrabajadores = new TrabajadorLog().Busqueda(new BuquedaTrabajador()).FirstOrDefault(r => r.IdTrabajador == obj.IdTrabajador);
            var objCargos = new CargoLog().Busqueda().FirstOrDefault(r => r.IdCargo == obj.IdCargo);
            var ObjSituacion = new SituacionLog().Busqueda().FirstOrDefault(r => r.IdSituacion == obj.IdSituacion);
            var objTpoDocumento = new TipoDocumentoLog().Busqueda().FirstOrDefault(r => r.IdTipoDocumento == objtrabajadores.IdTipoDocumento);
            var objSistemaPensiones = new SistemaPensionLog().Busqueda().FirstOrDefault(r => r.IdSistemaPension == obj.IdSistemaPension);

            // Aquí debes personalizar los datos dinámicos (puedes usar datos de tu base de datos)
            string html = $@"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Boleta de Pago</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; }}
                    .boleta {{ width: 800px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; }}
                    header {{ display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #000; padding-bottom: 10px; }}
                    header h1 {{ font-size: 24px; color: #0046ad; }}
                    header h1 span {{ color: orange; }}
                    .empresa-logo img {{ width: 150px; height: auto; }}
                    .trabajador-info, .detalles, .resumen {{ margin-top: 20px; }}
                    .detalles {{ display: flex; justify-content: space-between; }}
                    .detalles div {{ width: 30%; }}
                    .detalles table {{ width: 100%; border-collapse: collapse; }}
                    .detalles table td {{ padding: 5px; border-bottom: 1px solid #ccc; }}
                    footer {{ text-align: center; margin-top: 30px; }}
                </style>
            </head>
            <body>
                <div class='boleta'>
                    <header>
                        <div class='empresa-info'>
                            <h1>BOLETA DE PAGO <span>{obj.Mes}/{obj.Año}</span></h1>
                            <p><strong>Razón Social:</strong> Nombre Empresa Contratada</p>
                            <p><strong>Dirección:</strong> Direccion Empresa Contratada</p>
                            <p><strong>NIT:</strong> 25263987456 &nbsp; <strong>Reg. Patronal:</strong> 070710-00156</p>
                        </div>
                        <div class='empresa-logo'>
                            <img src='logo.png' alt='Logo de la Empresa'>
                            <p>D.S. N° 001-98-TR del 22/01/1998</p>
                        </div>
                    </header>

                    <section class='trabajador-info'>
                        <h2>Trabajador</h2>
                        <p><strong>Trabajador:</strong> {objtrabajadores.Documento} {objtrabajadores.Nombres} {objtrabajadores.ApellidoPaterno} {objtrabajadores.ApellidoMaterno}</p>
                        <p><strong>Fecha Ingreso:</strong> {objtrabajadores.FecIngreso.ToString("dd/MM/yyyy")}</p>
                        <p><strong>Cargo:</strong>{objCargos.Nombre}</p>
                        <p><strong>AFP/ONP:</strong> {objSistemaPensiones.Nombre} &nbsp; <strong>Código SPP:</strong> 652940CABEÑ3</p>
                        <p><strong>Días Trab.:</strong> {obj.nDiasTrab} &nbsp; <strong>Horas:</strong> {obj.nHorasNormal}</p>
                    </section>

                    <section class='detalles'>
                        <div class='ingresos'>
                            <h3>Ingresos</h3>
                            <table>
                                <tr><td>Rem. Básico</td><td>S/ {obj.HaberBasico}</td></tr>
                                <tr><td>Asig. Familiar</td><td>S/ {obj.vAsigFamiliar}</td></tr>
                                <tr><td>Horas Extras 25%</td><td>S/ {obj.vHorasExtra1}</td></tr>
                                <tr><td>Horas Extras 35%</td><td>S/ {obj.vHorasExtra2}</td></tr>
                                <tr><td>Dias Feriados</td><td>S/ {obj.vFeriadoTrab}</td></tr>
                                <tr><td>Vales</td><td>S/ {obj.ValesEmpleado}</td></tr>
                                <tr><td>Bonificación Cargo</td><td>S/ {obj.BonificacionCargo}</td></tr>
                                <tr><td>Total Ingresos</td><td><strong>S/ {obj.TotalIngreso}</strong></td></tr>
                            </table>
                        </div>
                        
                        <div class='descuentos'>
                            <h3>Descuentos de Ley</h3>
                            <table>
                                <tr><td>Aporte</td><td>S/ {obj.Aporte}</td></tr>
                                <tr><td>Comision</td><td>S/ {obj.Comision}</td></tr>
                                <tr><td>Prima</td><td>S/ {obj.Prima}</td></tr>
                                <tr><td>Total Descuentos</td><td><strong>S/ {obj.TotalDescuento}</strong></td></tr>
                            </table>
                        </div>

                        <div class='aportes'>
                            <h3>Aportes del Empleador</h3>
                            <table>
                                <tr><td>ESSALUD</td><td>S/ {obj.EsSalud}</td></tr>
                                <tr><td>Seguro Vida Ley</td><td>S/ {obj.SeguroVidaLey}</td></tr>
                                <tr><td>Total Empleador</td><td><strong>S/ {obj.EsSalud + obj.SeguroVidaLey}</strong></td></tr>
                            </table>
                        </div>
                    </section>

                    <section class='resumen'>
                        <div class='neto'>
                            <h3>Resumen</h3>
                            <p><strong>Neto a Pagar:</strong> S/ {obj.TotalNetoBoleta}</p>
                            <p><strong>Son:</strong> {obj.TotalNetoBoletaCad}</p>
                        </div>
                    </section>

                    <footer>
                        <p><strong>Emp. Nombre de Sistema</strong></p>
                        <p>Recibí Conforme: <span>____________</span> DNI: <span>____________</span></p>
                    </footer>
                </div>
            </body>
            </html>";

            return html;
        }
    }
} 