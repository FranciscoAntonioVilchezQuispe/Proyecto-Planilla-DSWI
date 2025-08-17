using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils.Response;

namespace Proyecto_Planilla_API.Data
{
    public class AsistenciaTrabajadorLog
    {
        public class GrabarAsistencias
        {
            public List<AsistenciasTrabajadores> Datos { get; set; }
            public int Año { get; set; }
            public int Mes { get; set; }
        }

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

        public bool InsertarLista(GrabarAsistencias asistencias)
        {
            if (asistencias.Datos == null || asistencias.Datos.Count == 0)
                return false;

            // Auditoría para cada registro
            foreach (var asistencia in asistencias.Datos)
            {
                new AuditoriaLog().SetAuditFieldsForInsert(asistencia);
            }

            // Eliminar registros existentes del mismo mes y año
            string deleteSql = "DELETE FROM AsistenciasTrabajadores WHERE Año = @Año AND Mes = @Mes";
            var deleteParams = new MySqlParameter[]
            {
        new MySqlParameter("@Año", asistencias.Año),
        new MySqlParameter("@Mes", asistencias.Mes)
            };
            ADOConnection.ExecuteNonQuery(deleteSql, deleteParams);

            // SQL para insertar registros
            string insertSql = @"
                                INSERT INTO AsistenciasTrabajadores (
                                    IdTrabajador, Año, Mes, DiasLaborales, DiasDescanso,
                                    DiasInasistencia, DiasFeriados, HorasExtra25, HorasExtra35,
                                    FecCreacion, Activo
                                ) VALUES (
                                    @IdTrabajador, @Año, @Mes, @DiasLaborales, @DiasDescanso,
                                    @DiasInasistencia, @DiasFeriados, @HorasExtra25, @HorasExtra35,
                                    @FecCreacion, @Activo
                                )";

            using (var connection = new MySqlConnection(ADOConnection.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var asistencia in asistencias.Datos)
                        {
                            using (var command = new MySqlCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddRange(new MySqlParameter[]
                                {
                            new MySqlParameter("@IdTrabajador", asistencia.IdTrabajador),
                            new MySqlParameter("@Año", asistencias.Año),
                            new MySqlParameter("@Mes", asistencias.Mes),
                            new MySqlParameter("@DiasLaborales", asistencia.DiasLaborales),
                            new MySqlParameter("@DiasDescanso", asistencia.DiasDescanso),
                            new MySqlParameter("@DiasInasistencia", asistencia.DiasInasistencia),
                            new MySqlParameter("@DiasFeriados", asistencia.DiasFeriados),
                            new MySqlParameter("@HorasExtra25", asistencia.HorasExtra25),
                            new MySqlParameter("@HorasExtra35", asistencia.HorasExtra35),
                            new MySqlParameter("@FecCreacion", asistencia.FecCreacion),
                            new MySqlParameter("@Activo", asistencia.Activo)
                                });

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Puedes loguear el error si lo deseas
                        throw new Exception("Error al insertar asistencias", ex);
                    }
                }
            }
        }

    }
}