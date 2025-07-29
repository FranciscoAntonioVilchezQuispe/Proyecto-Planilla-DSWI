using Microsoft.Data.SqlClient;
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
                              SELECT SCOPE_IDENTITY();";

            new AuditoriaLog().SetAuditFieldsForInsert(obj);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdTipoDocumento", obj.IdTipoDocumento),
                new SqlParameter("@Documento", obj.Documento),
                new SqlParameter("@Nombres", obj.Nombres),
                new SqlParameter("@ApellidoPaterno", obj.ApellidoPaterno),
                new SqlParameter("@ApellidoMaterno", obj.ApellidoMaterno),
                new SqlParameter("@IdGenero", obj.IdGenero),
                new SqlParameter("@IdEstadoCivil", obj.IdEstadoCivil),
                new SqlParameter("@Direccion", obj.Direccion),
                new SqlParameter("@Email", obj.Email),
                new SqlParameter("@Hijos", obj.Hijos),
                new SqlParameter("@IdCargo", obj.IdCargo),
                new SqlParameter("@FecNacimiento", obj.FecNacimiento),
                new SqlParameter("@FecIngreso", obj.FecIngreso),
                new SqlParameter("@IdSituacion", obj.IdSituacion),
                new SqlParameter("@IdSistemaPension", obj.IdSistemaPension),
                new SqlParameter("@Foto", obj.Foto ?? (object)DBNull.Value),
                new SqlParameter("@FecCreacion", obj.FecCreacion),
                new SqlParameter("@Activo", obj.Activo)
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

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdTrabajador", obj.IdTrabajador),
                new SqlParameter("@IdTipoDocumento", obj.IdTipoDocumento),
                new SqlParameter("@Documento", obj.Documento),
                new SqlParameter("@Nombres", obj.Nombres),
                new SqlParameter("@ApellidoPaterno", obj.ApellidoPaterno),
                new SqlParameter("@ApellidoMaterno", obj.ApellidoMaterno),
                new SqlParameter("@IdGenero", obj.IdGenero),
                new SqlParameter("@IdEstadoCivil", obj.IdEstadoCivil),
                new SqlParameter("@Direccion", obj.Direccion),
                new SqlParameter("@Email", obj.Email),
                new SqlParameter("@Hijos", obj.Hijos),
                new SqlParameter("@IdCargo", obj.IdCargo),
                new SqlParameter("@FecNacimiento", obj.FecNacimiento),
                new SqlParameter("@FecIngreso", obj.FecIngreso),
                new SqlParameter("@IdSituacion", obj.IdSituacion),
                new SqlParameter("@IdSistemaPension", obj.IdSistemaPension),
                new SqlParameter("@Foto", obj.Foto ?? (object)DBNull.Value),
                new SqlParameter("@FecUltimaModificacion", obj.FecUltimaModificacion)
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

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdTrabajador", id),
                new SqlParameter("@FecUltimaModificacion", DateTime.Now)
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

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BusquedaParam", obj.Busqueda ?? "")
            };

            var dataTable = ADOConnection.ExecuteDataTable(cadena, parameters);
            return ADOConnection.MapDataTableToList<Trabajadores>(dataTable);
        }
    }
} 