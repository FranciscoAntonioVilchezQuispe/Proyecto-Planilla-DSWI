using Microsoft.Data.SqlClient;
using Proyecto_Planilla_DSWI.Models;
using Proyecto_Planilla_DSWI.Utils.Response;

namespace Proyecto_Planilla_DSWI.Data
{
    public class AsistenciaTrabajadorLog
    {
        public List<AsistenciaTrabajadorResponse> BuscarAsistenciaByPeriodo(int año, int mes)
        {
            string cadena = $@"SELECT T.IdTrabajador, T.Documento, 
                                     CONCAT(t.ApellidoPaterno, ' ', t.ApellidoMaterno, ' ', t.Nombres) AS Nombre,
                                     a.DiasLaborales, a.DiasDescanso, a.DiasInasistencia, a.DiasFeriados, 
                                     a.HorasExtra25, a.HorasExtra35
                              FROM Trabajadores t
                              LEFT JOIN (SELECT * FROM AsistenciasTrabajadores WHERE Año = @Año AND Mes = @Mes) a 
                                   ON a.IdTrabajador = t.IdTrabajador";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Año", año),
                new SqlParameter("@Mes", mes)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<AsistenciaTrabajadorResponse>(dataTable);
        }

        public bool InsertarLista(List<AsistenciasTrabajadores> arr)
        {
            if (arr == null || arr.Count == 0)
                return false;

            // Configurar auditoría para todos los elementos
            foreach (var item in arr)
            {
                new AuditoriaLog().SetAuditFieldsForInsert(item);
            }

            // Eliminar registros existentes del período
            string deleteSql = "DELETE FROM AsistenciasTrabajadores WHERE Año = @Año AND Mes = @Mes";
            var deleteParameters = new SqlParameter[]
            {
                new SqlParameter("@Año", arr[0].Año),
                new SqlParameter("@Mes", arr[0].Mes)
            };

            ADOConnection.ExecuteNonQuery(deleteSql, deleteParameters);

            // Insertar nuevos registros
            string insertSql = $@"INSERT INTO AsistenciasTrabajadores
                                 (IdTrabajador, Año, Mes, DiasLaborales, DiasDescanso,
                                  DiasInasistencia, DiasFeriados, HorasExtra25, HorasExtra35,
                                  FecCreacion, Activo)
                                 VALUES
                                 (@IdTrabajador, @Año, @Mes, @DiasLaborales, @DiasDescanso,
                                  @DiasInasistencia, @DiasFeriados, @HorasExtra25, @HorasExtra35,
                                  @FecCreacion, @Activo)";

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
                                    new SqlParameter("@IdTrabajador", item.IdTrabajador),
                                    new SqlParameter("@Año", item.Año),
                                    new SqlParameter("@Mes", item.Mes),
                                    new SqlParameter("@DiasLaborales", item.DiasLaborales),
                                    new SqlParameter("@DiasDescanso", item.DiasDescanso),
                                    new SqlParameter("@DiasInasistencia", item.DiasInasistencia),
                                    new SqlParameter("@DiasFeriados", item.DiasFeriados),
                                    new SqlParameter("@HorasExtra25", item.HorasExtra25),
                                    new SqlParameter("@HorasExtra35", item.HorasExtra35),
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
    }
} 