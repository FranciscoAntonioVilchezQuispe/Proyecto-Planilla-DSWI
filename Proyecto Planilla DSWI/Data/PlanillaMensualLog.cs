using Microsoft.Data.SqlClient;
using Proyecto_Planilla_DSWI.Models;
using Proyecto_Planilla_DSWI.Utils.Request;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class PlanillaMensualLog
    {
        public List<PlanillaMensual> CalcularPlanillaByPeriodo(int año, int mes)
        {
            List<PlanillaMensual> arr = new List<PlanillaMensual>();
            var parametro = new ParametrosLog().BusquedaOne();
            var Arrtrabajador = new TrabajadorLog().Busqueda(new BuquedaTrabajador { Busqueda = "", Estado = _Estado.Activo });
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
                obj.TotalNetoBoletaCad = Utils.NumberToLetters.ToCardinal((decimal)obj.TotalNetoBoleta) + " SOLES";
                arr.Add(obj);
            }

            return arr;
        }

        public bool InsertarLista(List<PlanillaMensual> arr)
        {
            if (arr == null || arr.Count == 0)
                return false;

            // Configurar auditoría para todos los elementos
            foreach (var item in arr)
            {
                new AuditoriaLog().SetAuditFieldsForInsert(item);
            }

            // Eliminar registros existentes del período
            string deleteSql = "DELETE FROM PlanillaMensual WHERE Año = @Año AND Mes = @Mes";
            var deleteParameters = new SqlParameter[]
            {
                new SqlParameter("@Año", arr[0].Año),
                new SqlParameter("@Mes", arr[0].Mes)
            };

            ADOConnection.ExecuteNonQuery(deleteSql, deleteParameters);

            // Insertar nuevos registros
            string insertSql = $@"INSERT INTO PlanillaMensual
                                 (Año, Mes, IdTrabajador, IdSituacion, IdCargo, Apellido, Nombre, IdSistemaPension, IdEstadoCivil,
                                  Hijos, FechaIngreso, SueldoBasico, PorcHoraExtra1, PorcHoraExtra2, PorcDescansoTrab, PorcFeriadoTrab, 
                                  PorcAsigFamiliar, nHorasNormal, nHorasExtra1, nHorasExtra2, nDiasTrab, nDiasDescansos, nFeriadosTrab, 
                                  nDescansosTrab, nDiasInasistencias, HaberBasico, ValesEmpleado, vHorasExtra1, vHorasExtra2, vAsigFamiliar, 
                                  vDescansoTrab, vFeriadoTrab, BonificacionCargo, BonificacionMovilidad, CanastaNavidad, Escolaridad,
                                  DiaTrabajador, TotalIngreso, PorcAporte, Aporte, PorcComision, Comision, PorcPrima, Prima, 
                                  TotalDescuento, TotalNetoBoleta, TotalNetoBoletaCad, FecCreacion, Activo)
                                 VALUES
                                 (@Año, @Mes, @IdTrabajador, @IdSituacion, @IdCargo, @Apellido, @Nombre, @IdSistemaPension, @IdEstadoCivil,
                                  @Hijos, @FechaIngreso, @SueldoBasico, @PorcHoraExtra1, @PorcHoraExtra2, @PorcDescansoTrab, @PorcFeriadoTrab,
                                  @PorcAsigFamiliar, @nHorasNormal, @nHorasExtra1, @nHorasExtra2, @nDiasTrab, @nDiasDescansos, @nFeriadosTrab,
                                  @nDescansosTrab, @nDiasInasistencias, @HaberBasico, @ValesEmpleado, @vHorasExtra1, @vHorasExtra2, @vAsigFamiliar,
                                  @vDescansoTrab, @vFeriadoTrab, @BonificacionCargo, @BonificacionMovilidad, @CanastaNavidad, @Escolaridad,
                                  @DiaTrabajador, @TotalIngreso, @PorcAporte, @Aporte, @PorcComision, @Comision, @PorcPrima, @Prima,
                                  @TotalDescuento, @TotalNetoBoleta, @TotalNetoBoletaCad, @FecCreacion, @Activo)";

            using (var connection = new SqlConnection(ADOConnection.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in arr)
                        {
                            using (var command = new SqlCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddRange(new SqlParameter[]
                                {
                                    new SqlParameter("@Año", item.Año),
                                    new SqlParameter("@Mes", item.Mes),
                                    new SqlParameter("@IdTrabajador", item.IdTrabajador),
                                    new SqlParameter("@IdSituacion", item.IdSituacion),
                                    new SqlParameter("@IdCargo", item.IdCargo),
                                    new SqlParameter("@Apellido", item.Apellido),
                                    new SqlParameter("@Nombre", item.Nombre),
                                    new SqlParameter("@IdSistemaPension", item.IdSistemaPension),
                                    new SqlParameter("@IdEstadoCivil", item.IdEstadoCivil),
                                    new SqlParameter("@Hijos", item.Hijos),
                                    new SqlParameter("@FechaIngreso", item.FechaIngreso),
                                    new SqlParameter("@SueldoBasico", item.SueldoBasico),
                                    new SqlParameter("@PorcHoraExtra1", item.PorcHoraExtra1),
                                    new SqlParameter("@PorcHoraExtra2", item.PorcHoraExtra2),
                                    new SqlParameter("@PorcDescansoTrab", item.PorcDescansoTrab),
                                    new SqlParameter("@PorcFeriadoTrab", item.PorcFeriadoTrab),
                                    new SqlParameter("@PorcAsigFamiliar", item.PorcAsigFamiliar),
                                    new SqlParameter("@nHorasNormal", item.nHorasNormal),
                                    new SqlParameter("@nHorasExtra1", item.nHorasExtra1),
                                    new SqlParameter("@nHorasExtra2", item.nHorasExtra2),
                                    new SqlParameter("@nDiasTrab", item.nDiasTrab),
                                    new SqlParameter("@nDiasDescansos", item.nDiasDescansos),
                                    new SqlParameter("@nFeriadosTrab", item.nFeriadosTrab),
                                    new SqlParameter("@nDescansosTrab", item.nDescansosTrab),
                                    new SqlParameter("@nDiasInasistencias", item.nDiasInasistencias),
                                    new SqlParameter("@HaberBasico", item.HaberBasico),
                                    new SqlParameter("@ValesEmpleado", item.ValesEmpleado),
                                    new SqlParameter("@vHorasExtra1", item.vHorasExtra1),
                                    new SqlParameter("@vHorasExtra2", item.vHorasExtra2),
                                    new SqlParameter("@vAsigFamiliar", item.vAsigFamiliar),
                                    new SqlParameter("@vDescansoTrab", item.vDescansoTrab),
                                    new SqlParameter("@vFeriadoTrab", item.vFeriadoTrab),
                                    new SqlParameter("@BonificacionCargo", item.BonificacionCargo),
                                    new SqlParameter("@BonificacionMovilidad", item.BonificacionMovilidad),
                                    new SqlParameter("@CanastaNavidad", item.CanastaNavidad),
                                    new SqlParameter("@Escolaridad", item.Escolaridad),
                                    new SqlParameter("@DiaTrabajador", item.DiaTrabajador),
                                    new SqlParameter("@TotalIngreso", item.TotalIngreso),
                                    new SqlParameter("@PorcAporte", item.PorcAporte),
                                    new SqlParameter("@Aporte", item.Aporte),
                                    new SqlParameter("@PorcComision", item.PorcComision),
                                    new SqlParameter("@Comision", item.Comision),
                                    new SqlParameter("@PorcPrima", item.PorcPrima),
                                    new SqlParameter("@Prima", item.Prima),
                                    new SqlParameter("@TotalDescuento", item.TotalDescuento),
                                    new SqlParameter("@TotalNetoBoleta", item.TotalNetoBoleta),
                                    new SqlParameter("@TotalNetoBoletaCad", item.TotalNetoBoletaCad),
                                    new SqlParameter("@FecCreacion", item.FecCreacion),
                                    new SqlParameter("@Activo", item.Activo)
                                });
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<PlanillaMensual> Lista(int año, int mes)
        {
            string cadena = $@"SELECT * FROM PlanillaMensual WHERE Año = @Año AND Mes = @Mes";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Año", año),
                new SqlParameter("@Mes", mes)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<PlanillaMensual>(dataTable);
        }

        public string BuscarBoleta(BusquedaBoleta objboleta)
        {
            string cadena = $@"SELECT * FROM PlanillaMensual 
                              WHERE Año = @Año AND Mes = @Mes AND IdTrabajador IN 
                              (SELECT IdTrabajador FROM Trabajadores WHERE Documento = @Documento)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Año", objboleta.Año),
                new SqlParameter("@Mes", objboleta.Mes),
                new SqlParameter("@Documento", objboleta.Documento)
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