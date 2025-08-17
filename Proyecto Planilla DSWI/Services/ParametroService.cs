using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class ParametroService : IParametroService
    {
        private readonly string _apiBaseUrl;

        public ParametroService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(Parametros parametro)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Parametros, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/ParametrosApi/Insert",
                    parametro
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

        public async Task<IToReturn<int>> UpdateAsync(int id, Parametros parametro)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Parametros, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/ParametrosApi/Update/{id}",
                    parametro
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
                    "/api/ParametrosApi/CambiarEstado",
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

        public async Task<IToReturn<Parametros>> BusquedaOneAsync()
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, Parametros>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/ParametrosApi/BusquedaOne",
                    null
                );

                return new ToReturn<Parametros>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Parametros>(ex.Message),
                    _ => new ToReturnError<Parametros>(ex.Message)
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

            return 500; // Default
        }
    }
}