using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class SistemaPensionService : ISistemaPensionService
    {
        private readonly string _apiBaseUrl;

        public SistemaPensionService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(SistemaPensiones sistemaPension)
        {
            try
            {
                // Usar tu clase ToResponse para hacer la llamada
                var result = await ToResponse.HTTPExecuteAsync<SistemaPensiones, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/SistemaPensionApi/Insert",
                    sistemaPension
                );

                return new ToReturn<int>(result);
            }
            catch (Exception ex)
            {
                // Extraer el status code del mensaje si es posible
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    412 => new ToReturnValidation<int>(ex.Message),
                    404 => new ToReturnNoEncontrado<int>(ex.Message),
                    _ => new ToReturnError<int>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<int>> UpdateAsync(int id, SistemaPensiones sistemaPension)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<SistemaPensiones, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/SistemaPensionApi/Update/{id}",
                    sistemaPension
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
                    "/api/SistemaPensionApi/CambiarEstado",
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

        public async Task<IToReturn<Paginacion<List<SistemaPensiones>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<SistemaPensiones>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/SistemaPensionApi/BusquedaPaginada",
                   new BusquedaPaginacion { page = page, pageSize = pageSize, estado = estado }
                );

                return new ToReturn<Paginacion<List<SistemaPensiones>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<SistemaPensiones>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<SistemaPensiones>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturnList<SistemaPensiones>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<SistemaPensiones>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/SistemaPensionApi",
                    $"Busqueda?estado={estado}"
                );

                return new ToReturnList<SistemaPensiones>(result ?? new List<SistemaPensiones>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<SistemaPensiones>(ex.Message),
                    _ => new ToReturnErrorList<SistemaPensiones>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<SistemaPensiones>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, SistemaPensiones>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/SistemaPensionApi/GetById",
                    id
                );

                return new ToReturn<SistemaPensiones>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<SistemaPensiones>(ex.Message),
                    _ => new ToReturnError<SistemaPensiones>(ex.Message)
                };
            }
        }

        // Método helper para extraer el código de estado de la excepción
        private int ExtractStatusCodeFromException(Exception ex)
        {
            var message = ex.Message;

            if (message.Contains("Error 412:"))
                return 412;
            if (message.Contains("Error 404:"))
                return 404;
            if (message.Contains("Error 401:"))
                return 401;

            return 500; // Default
        }
    }
}
