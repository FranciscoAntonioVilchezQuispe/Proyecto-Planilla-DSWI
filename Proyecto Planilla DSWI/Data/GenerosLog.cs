using Microsoft.Data.SqlClient;
using Proyecto_Planilla_DSWI.Models;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class GenerosLog
    {
        public int Insert(Generos obj)
        {
            string cadena = $@"INSERT INTO Generos
                              (Nombre, FecCreacion, Activo)
                              VALUES (@Nombre, @FecCreacion, @Activo);
                              SELECT SCOPE_IDENTITY();";

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@FecCreacion", obj.FecCreacion),
                new SqlParameter("@Activo", obj.Activo)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public int Update(Generos obj)
        {
            string cadena = $@"UPDATE Generos 
                              SET Nombre = @Nombre, 
                                  FecUltimaModificacion = @FecUltimaModificacion 
                              WHERE IdGenero = @IdGenero";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdGenero", obj.IdGenero),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE Generos
                              SET Activo = CASE 
                                           WHEN Activo = 1 THEN 0 
                                           ELSE 1 
                                           END,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdGenero = @IdGenero;
                              SELECT 1;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdGenero", id),
                new SqlParameter("@FecUltimaModificacion", DateTime.Now)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public IEnumerable<Generos> Busqueda(_Estado estado = _Estado.Todos)
        {
            string cadena = $@"SELECT * FROM Generos 
                              {(estado != _Estado.Todos ? $@" WHERE Activo = {(int)estado}" : "")}";

            var dataTable = ADOConnection.ExecuteDataTable(cadena);
            return ADOConnection.MapDataTableToList<Generos>(dataTable);
        }
    }
} 