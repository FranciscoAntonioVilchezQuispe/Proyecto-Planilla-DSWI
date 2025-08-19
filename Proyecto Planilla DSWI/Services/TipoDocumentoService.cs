using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly string _apiBaseUrl;

        public TipoDocumentoService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(TipoDocumentos tipoDocumento)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<TipoDocumentos, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/TipoDocumentoApi/Insert",
                    tipoDocumento
                );

                return new ToReturn<int>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    412 => new ToReturnValidation<int>(ex.Message),
                    404 => new ToReturnNoEncontrado<int>(ex.Message),
                    _ => new ToReturnError<int>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<int>> UpdateAsync(int id, TipoDocumentos tipoDocumento)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<TipoDocumentos, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/TipoDocumentoApi/Update/{id}",
                    tipoDocumento
                );

                return new ToReturn<int>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    412 => new ToReturnValidation<int>(ex.Message),
                    404 => new ToReturnNoEncontrado<int>(ex.Message),
                    _ => new ToReturnError<int>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<int>> CambiarEstadoAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, int>(
                    Metodo.DELETE,
                    _apiBaseUrl,
                    "/api/TipoDocumentoApi/CambiarEstado",
                    id
                );

                return new ToReturn<int>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<int>(ex.Message),
                    _ => new ToReturnError<int>(ex.Message)
                };
            }
        }

        public async Task<IToReturnList<TipoDocumentos>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<TipoDocumentos>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/TipoDocumentoApi/Busqueda?estado={estado}",
                    null
                );

                return new ToReturnList<TipoDocumentos>(result ?? new List<TipoDocumentos>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<TipoDocumentos>(ex.Message),
                    _ => new ToReturnErrorList<TipoDocumentos>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Paginacion<List<TipoDocumentos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<TipoDocumentos>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/TipoDocumentoApi/BusquedaPaginada",
                   new BusquedaPaginacion { page = page, pageSize = pageSize, estado = estado }
                );

                return new ToReturn<Paginacion<List<TipoDocumentos>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<TipoDocumentos>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<TipoDocumentos>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<TipoDocumentos>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, TipoDocumentos>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/TipoDocumentoApi/GetById",
                    id
                );

                return new ToReturn<TipoDocumentos>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<TipoDocumentos>(ex.Message),
                    _ => new ToReturnError<TipoDocumentos>(ex.Message)
                };
            }
        }

        private int ExtractStatusCodeFromException(Exception ex)
        {
            var message = ex.Message;

            if (message.Contains("Error 412:"))
                return 412;
            if (message.Contains("Error 404:"))
                return 404;
            if (message.Contains("Error 401:"))
                return 401;

            return 500;
        }
    }
}
