using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class IngresosTrabajadoresService : IIngresosTrabajadoresService
    {
        private readonly string _apiBaseUrl;

        public IngresosTrabajadoresService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturnList<IngresosTrabajadores>> BusquedaAsync(GlobalEnum._Estado estado = GlobalEnum._Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<IngresosTrabajadores>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/TrabajadorApi/Busqueda",
                    "Busqueda?estado={estado}"
                );

                return new ToReturnList<IngresosTrabajadores>(result ?? new List<IngresosTrabajadores>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<IngresosTrabajadores>(ex.Message),
                    _ => new ToReturnErrorList<IngresosTrabajadores>(ex.Message)
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
                    "/api/IngresosTrabajadoresApi/CambiarEstado",
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

        public async Task<IToReturn<IngresosTrabajadores>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, IngresosTrabajadores>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/IngresosTrabajadoresApi/GetById",
                    id
                );

                return new ToReturn<IngresosTrabajadores>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<IngresosTrabajadores>(ex.Message),
                    _ => new ToReturnError<IngresosTrabajadores>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<int>> InsertAsync(IngresosTrabajadores trabajadores)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<IngresosTrabajadores, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/IngresosTrabajadoresApi/Insert",
                    trabajadores
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

        public async Task<IToReturn<int>> UpdateAsync(int id, IngresosTrabajadores trabajadores)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<IngresosTrabajadores, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/IngresosTrabajadoresApi/Update/{id}",
                    trabajadores
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
