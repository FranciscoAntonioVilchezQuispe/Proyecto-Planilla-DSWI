using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils.Response;

namespace Proyecto_Planilla_API.Data
{
    public class AsistenciaTrabajadorLog
    {


        public List<AsistenciaTrabajadorResponse> BuscarAsistenciaByPeriodo(int año, int mes)
        {
            string cadena = @"SELECT 
                        t.IdTrabajador, 
                        t.Documento, 
                        CONCAT(t.ApellidoPaterno, ' ', t.ApellidoMaterno, ' ', t.Nombres) AS Nombre,
                        IFNULL(a.DiasLaborales, 0) AS DiasLaborales,
                        IFNULL(a.DiasDescanso, 0) AS DiasDescanso,
                        IFNULL(a.DiasInasistencia, 0) AS DiasInasistencia,
                        IFNULL(a.DiasFeriados, 0) AS DiasFeriados,
                        IFNULL(a.HorasExtra25, 0.0) AS HorasExtra25,
                        IFNULL(a.HorasExtra35, 0.0) AS HorasExtra35
                      FROM Trabajadores t
                      LEFT JOIN AsistenciasTrabajadores a 
                        ON a.IdTrabajador = t.IdTrabajador 
                        AND a.Año = @Año 
                        AND a.Mes = @Mes";


            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Año", año),
                new MySqlParameter("@Mes", mes)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<AsistenciaTrabajadorResponse>(dataTable);
        }

        public bool InsertarLista(List<AsistenciasTrabajadores> arr)
        {
            if (arr == null || arr.Count == 0)
                return false;

         
            foreach (var item in arr)
            {
                new AuditoriaLog().SetAuditFieldsForInsert(item);
            }

  
            string deleteSql = "DELETE FROM AsistenciasTrabajadores WHERE Año = @Año AND Mes = @Mes";
            var deleteParameters = new MySqlParameter[]
            {
                new MySqlParameter("@Año", arr[0].Año),
                new MySqlParameter("@Mes", arr[0].Mes)
            };

            ADOConnection.ExecuteNonQuery(deleteSql, deleteParameters);

  
            string insertSql = $@"INSERT INTO AsistenciasTrabajadores
                                 (IdTrabajador, Año, Mes, DiasLaborales, DiasDescanso,
                                  DiasInasistencia, DiasFeriados, HorasExtra25, HorasExtra35,
                                  FecCreacion, Activo)
                                 VALUES
                                 (@IdTrabajador, @Año, @Mes, @DiasLaborales, @DiasDescanso,
                                  @DiasInasistencia, @DiasFeriados, @HorasExtra25, @HorasExtra35,
                                  @FecCreacion, @Activo)";

            using (var connection = new MySqlConnection(ADOConnection.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in arr)
                        {
                            using (var command = new MySqlCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddRange(new MySqlParameter[]
                                {
                                    new MySqlParameter("@IdTrabajador", item.IdTrabajador),
                                    new MySqlParameter("@Año", item.Año),
                                    new MySqlParameter("@Mes", item.Mes),
                                    new MySqlParameter("@DiasLaborales", item.DiasLaborales),
                                    new MySqlParameter("@DiasDescanso", item.DiasDescanso),
                                    new MySqlParameter("@DiasInasistencia", item.DiasInasistencia),
                                    new MySqlParameter("@DiasFeriados", item.DiasFeriados),
                                    new MySqlParameter("@HorasExtra25", item.HorasExtra25),
                                    new MySqlParameter("@HorasExtra35", item.HorasExtra35),
                                    new MySqlParameter("@FecCreacion", item.FecCreacion),
                                    new MySqlParameter("@Activo", item.Activo)
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