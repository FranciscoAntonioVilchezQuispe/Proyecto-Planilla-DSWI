using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class IngresosTrabajadoresLog
    {
        public int Insert(IngresosTrabajadores obj)
        {
            string cadena = $@"INSERT INTO IngresosTrabajadores
                              (IdTrabajador, Remuneracion, Vale, BonifCargo, FecCreacion, Activo)
                              VALUES (@IdTrabajador, @Remuneracion, @Vale, @BonifCargo, @FecCreacion, @Activo);
                              SELECT LAST_INSERT_ID();";  // Cambiado SCOPE_IDENTITY() por LAST_INSERT_ID()

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTrabajador", obj.IdTrabajador),
                new MySqlParameter("@Remuneracion", obj.Remuneracion),
                new MySqlParameter("@Vale", obj.Vale),
                new MySqlParameter("@BonifCargo", obj.BonifCargo),
                new MySqlParameter("@FecCreacion", obj.FecCreacion),
                new MySqlParameter("@Activo", obj.Activo)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public int Update(IngresosTrabajadores obj)
        {
            string cadena = $@"UPDATE IngresosTrabajadores 
                              SET Remuneracion = @Remuneracion,
                                  Vale = @Vale,
                                  BonifCargo = @BonifCargo,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdTrabajador = @IdTrabajador";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTrabajador", obj.IdTrabajador),
                new MySqlParameter("@Remuneracion", obj.Remuneracion),
                new MySqlParameter("@Vale", obj.Vale),
                new MySqlParameter("@BonifCargo", obj.BonifCargo),
                new MySqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE IngresosTrabajadores
                              SET Activo = CASE 
                                           WHEN Activo = 1 THEN 0 
                                           ELSE 1 
                                           END,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdTrabajador = @IdTrabajador;
                              SELECT 1;";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTrabajador", id),
                new MySqlParameter("@FecUltimaModificacion", DateTime.Now)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public IngresosTrabajadores BusquedaOne(int id)
        {
            string cadena = $@"SELECT TOP 1 * FROM IngresosTrabajadores WHERE IdTrabajador = @IdTrabajador";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTrabajador", id)
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            if (dataTable.Rows.Count > 0)
            {
                return ADOConnection.MapDataRowToObject<IngresosTrabajadores>(dataTable.Rows[0]);
            }
            return null;
        }

        public IEnumerable<IngresosTrabajadores> Busqueda(_Estado estado = _Estado.Todos)
        {
            string cadena = $@"SELECT * FROM IngresosTrabajadores 
                              {(estado != _Estado.Todos ? $@" WHERE Activo = {(int)estado}" : "")}";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            return ADOConnection.MapDataTableToList<IngresosTrabajadores>(dataTable);
        }
    }
} 