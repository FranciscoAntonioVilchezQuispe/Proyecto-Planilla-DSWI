using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class CargoLog
    {
        public int Insert(Cargos obj)
        {
            string cadena = $@"INSERT INTO Cargos
                              (Nombre, FecCreacion, Activo)
                              VALUES (@Nombre, @FecCreacion, @Activo);
                              SELECT LAST_INSERT_ID();";  // Cambiado SCOPE_IDENTITY() por LAST_INSERT_ID()

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Nombre", obj.Nombre),
                new MySqlParameter("@FecCreacion", obj.FecCreacion),
                new MySqlParameter("@Activo", obj.Activo)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public int Update(Cargos obj)
        {
            string cadena = $@"UPDATE Cargos 
                              SET Nombre = @Nombre, 
                                  FecUltimaModificacion = @FecUltimaModificacion 
                              WHERE IdCargo = @IdCargo";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdCargo", obj.IdCargo),
                new MySqlParameter("@Nombre", obj.Nombre),
                new MySqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE Cargos
                              SET Activo = CASE 
                                           WHEN Activo = 1 THEN 0 
                                           ELSE 1 
                                           END,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdCargo = @IdCargo;
                              SELECT 1;";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdCargo", id),
                new MySqlParameter("@FecUltimaModificacion", DateTime.Now)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public IEnumerable<Cargos> Busqueda(_Estado estado = _Estado.Todos)
        {
            string cadena = $@"SELECT * FROM Cargos 
                              {(estado != _Estado.Todos ? $@" WHERE Activo = {(int)estado}" : "")}";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            return ADOConnection.MapDataTableToList<Cargos>(dataTable);
        }
    }
} 