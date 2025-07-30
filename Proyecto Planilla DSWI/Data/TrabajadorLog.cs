using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Proyecto_Planilla_DSWI.Models;
using Proyecto_Planilla_DSWI.Utils.Request;
using static Proyecto_Planilla_DSWI.Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Data
{
    public class TrabajadorLog
    {
        public int Insert(Trabajadores obj)
        {
            string cadena = $@"INSERT INTO Trabajadores
                              (IdTipoDocumento, Documento, Nombres, ApellidoPaterno, ApellidoMaterno, IdGenero,
                               IdEstadoCivil, Direccion, Email, Hijos, IdCargo, FecNacimiento,
                               FecIngreso, IdSituacion, IdSistemaPension, Foto, FecCreacion, Activo)
                              VALUES (@IdTipoDocumento, @Documento, @Nombres, @ApellidoPaterno, @ApellidoMaterno, @IdGenero,
                                     @IdEstadoCivil, @Direccion, @Email, @Hijos, @IdCargo, @FecNacimiento,
                                     @FecIngreso, @IdSituacion, @IdSistemaPension, @Foto, @FecCreacion, @Activo);
                              SELECT LAST_INSERT_ID();";  // Cambiado SCOPE_IDENTITY() por LAST_INSERT_ID()

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTipoDocumento", obj.IdTipoDocumento),
                new MySqlParameter("@Documento", obj.Documento),
                new MySqlParameter("@Nombres", obj.Nombres),
                new MySqlParameter("@ApellidoPaterno", obj.ApellidoPaterno),
                new MySqlParameter("@ApellidoMaterno", obj.ApellidoMaterno),
                new MySqlParameter("@IdGenero", obj.IdGenero),
                new MySqlParameter("@IdEstadoCivil", obj.IdEstadoCivil),
                new MySqlParameter("@Direccion", obj.Direccion),
                new MySqlParameter("@Email", obj.Email),
                new MySqlParameter("@Hijos", obj.Hijos),
                new MySqlParameter("@IdCargo", obj.IdCargo),
                new MySqlParameter("@FecNacimiento", obj.FecNacimiento),
                new MySqlParameter("@FecIngreso", obj.FecIngreso),
                new MySqlParameter("@IdSituacion", obj.IdSituacion),
                new MySqlParameter("@IdSistemaPension", obj.IdSistemaPension),
                new MySqlParameter("@Foto", obj.Foto ?? (object)DBNull.Value),
                new MySqlParameter("@FecCreacion", obj.FecCreacion),
                new MySqlParameter("@Activo", obj.Activo)
            };

            return ADOConnection.ExecuteInt(cadena, parameters);
        }

        public int Update(Trabajadores obj)
        {
            string cadena = $@"UPDATE Trabajadores
                              SET IdTipoDocumento = @IdTipoDocumento,
                                  Documento = @Documento,
                                  Nombres = @Nombres,
                                  ApellidoPaterno = @ApellidoPaterno,
                                  ApellidoMaterno = @ApellidoMaterno,
                                  IdGenero = @IdGenero,
                                  IdEstadoCivil = @IdEstadoCivil,
                                  Direccion = @Direccion,
                                  Email = @Email,
                                  Hijos = @Hijos,
                                  IdCargo = @IdCargo,
                                  FecNacimiento = @FecNacimiento,
                                  FecIngreso = @FecIngreso,
                                  IdSituacion = @IdSituacion,
                                  IdSistemaPension = @IdSistemaPension,
                                  Foto = @Foto,
                                  FecUltimaModificacion = @FecUltimaModificacion
                              WHERE IdTrabajador = @IdTrabajador";

            new AuditoriaLog().SetAuditFieldsForUpdate(obj);

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@IdTrabajador", obj.IdTrabajador),
                new MySqlParameter("@IdTipoDocumento", obj.IdTipoDocumento),
                new MySqlParameter("@Documento", obj.Documento),
                new MySqlParameter("@Nombres", obj.Nombres),
                new MySqlParameter("@ApellidoPaterno", obj.ApellidoPaterno),
                new MySqlParameter("@ApellidoMaterno", obj.ApellidoMaterno),
                new MySqlParameter("@IdGenero", obj.IdGenero),
                new MySqlParameter("@IdEstadoCivil", obj.IdEstadoCivil),
                new MySqlParameter("@Direccion", obj.Direccion),
                new MySqlParameter("@Email", obj.Email),
                new MySqlParameter("@Hijos", obj.Hijos),
                new MySqlParameter("@IdCargo", obj.IdCargo),
                new MySqlParameter("@FecNacimiento", obj.FecNacimiento),
                new MySqlParameter("@FecIngreso", obj.FecIngreso),
                new MySqlParameter("@IdSituacion", obj.IdSituacion),
                new MySqlParameter("@IdSistemaPension", obj.IdSistemaPension),
                new MySqlParameter("@Foto", obj.Foto ?? (object)DBNull.Value),
                new MySqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
            };

            return ADOConnection.ExecuteNonQuery(cadena, parameters) ? 1 : 0;
        }

        public int CambiarEstado(int id)
        {
            string cadena = $@"UPDATE Trabajadores
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

        public IEnumerable<Trabajadores> Busqueda(BuquedaTrabajador obj)
        {
            string cadena = $@"DECLARE @busqueda VARCHAR(50);
                              SET @busqueda = @BusquedaParam;
                              
                              WITH Palabras AS (
                                  SELECT value AS palabra
                                  FROM STRING_SPLIT(@busqueda, ' ')
                              )
                              SELECT *
                              FROM Trabajadores
                              WHERE (
                                  -- Si @busqueda está vacío, devolver todos los registros
                                  @busqueda IS NULL OR @busqueda = ''
                              )
                              OR EXISTS (
                                  -- Verificar que todas las palabras del término de búsqueda coincidan en algún campo
                                  SELECT 1
                                  FROM Palabras
                                  WHERE 
                                      (ApellidoPaterno LIKE '%' + palabra + '%'
                                      OR ApellidoMaterno LIKE '%' + palabra + '%'
                                      OR Nombres LIKE '%' + palabra + '%'
                                      OR Documento LIKE '%' + palabra + '%')
                              ) {(obj.Estado != _Estado.Todos ? $@" AND Activo = {(int)obj.Estado}" : "")}";

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@BusquedaParam", obj.Busqueda ?? "")
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<Trabajadores>(dataTable);
        }
    }
} 