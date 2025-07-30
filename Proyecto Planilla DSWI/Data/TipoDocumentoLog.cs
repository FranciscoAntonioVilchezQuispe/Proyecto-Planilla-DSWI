using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class TipoDocumentoLog
    {
        public int Insert(TipoDocumentos obj)
        {
            string cadena = $@"INSERT INTO TipoDocumentos
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

        public int Update(TipoDocumentos obj)
        {
            string cadena = $@"UPDATE TipoDocumentos 
                              SET Nombre = @Nombre, 
                                  FecUltimaModificacion = @FecUltimaModificacion 
                              WHERE IdTipoDocumento = @IdTipoDocumento";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTipoDocumento", obj.IdTipoDocumento),
                new MySqlParameter("@Nombre", obj.Nombre),
                new MySqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE TipoDocumentos
                              SET Activo = CASE 
                                           WHEN Activo = 1 THEN 0 
                                           ELSE 1 
                                           END,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdTipoDocumento = @IdTipoDocumento;
                              SELECT 1;";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTipoDocumento", id),
                new MySqlParameter("@FecUltimaModificacion", DateTime.Now)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public IEnumerable<TipoDocumentos> Busqueda(_Estado estado = _Estado.Todos)
        {
            string cadena = $@"SELECT * FROM TipoDocumentos 
                              {(estado != _Estado.Todos ? $@" WHERE Activo = {(int)estado}" : "")}";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            return ADOConnection.MapDataTableToList<TipoDocumentos>(dataTable);
        }
    }
} 