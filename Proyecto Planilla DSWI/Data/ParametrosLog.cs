using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class ParametrosLog
    {
        public int Insert(Parametros obj)
        {
            string cadena = $@"INSERT INTO Parametros
                              (RemBasico, PorcAsigancionFamiliar, PorcExtra1, PorcExtra2, 
                               FecCreacion, Activo)
                              VALUES
                              (@RemBasico, @PorcAsigancionFamiliar, @PorcExtra1, @PorcExtra2,
                               @FecCreacion, @Activo);
                               SELECT LAST_INSERT_ID();";  // Cambiado SCOPE_IDENTITY() por LAST_INSERT_ID()

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@RemBasico", obj.RemBasico),
                new MySqlParameter("@PorcAsigancionFamiliar", obj.PorcAsigancionFamiliar),
                new MySqlParameter("@PorcExtra1", obj.PorcExtra1),
                new MySqlParameter("@PorcExtra2", obj.PorcExtra2),
                new MySqlParameter("@FecCreacion", obj.FecCreacion),
                new MySqlParameter("@Activo", obj.Activo)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public int Update(Parametros obj)
        {
            string cadena = $@"UPDATE Parametros
                              SET RemBasico = @RemBasico,
                                  PorcAsigancionFamiliar = @PorcAsigancionFamiliar,
                                  PorcExtra1 = @PorcExtra1,
                                  PorcExtra2 = @PorcExtra2,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdParametro = @IdParametro";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdParametro", obj.IdParametro),
                new MySqlParameter("@RemBasico", obj.RemBasico),
                new MySqlParameter("@PorcAsigancionFamiliar", obj.PorcAsigancionFamiliar),
                new MySqlParameter("@PorcExtra1", obj.PorcExtra1),
                new MySqlParameter("@PorcExtra2", obj.PorcExtra2),
                new MySqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE Parametros
                              SET Activo = CASE 
                                           WHEN Activo = 1 THEN 0 
                                           ELSE 1 
                                           END,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdParametro = @IdParametro;
                              SELECT 1;";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdParametro", id),
                new MySqlParameter("@FecUltimaModificacion", DateTime.Now)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public IEnumerable<Parametros> Busqueda(_Estado estado = _Estado.Todos)
        {
            string cadena = $@"SELECT * FROM Parametros 
                              {(estado != _Estado.Todos ? $@" WHERE Activo = {(int)estado}" : "")}";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            return ADOConnection.MapDataTableToList<Parametros>(dataTable);
        }

        public Parametros BusquedaOne()
        {
            string cadena = $@"SELECT TOP 1 * FROM Parametros";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            if (dataTable.Rows.Count > 0)
            {
                return ADOConnection.MapDataRowToObject<Parametros>(dataTable.Rows[0]);
            }
            return null;
        }
    }
} 