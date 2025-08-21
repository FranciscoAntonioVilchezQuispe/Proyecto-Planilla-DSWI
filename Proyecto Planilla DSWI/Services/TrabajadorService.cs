using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly string _apiBaseUrl;

        public TrabajadorService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturnList<Trabajadores>> BusquedaAsync(string busqueda, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BuquedaTrabajador, List<Trabajadores>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/TrabajadorApi/Busqueda",
                    new BuquedaTrabajador { Busqueda = busqueda, Estado = estado }
                );

                return new ToReturnList<Trabajadores>(result ?? new List<Trabajadores>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<Trabajadores>(ex.Message),
                    _ => new ToReturnErrorList<Trabajadores>(ex.Message)
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
                    "/api/TrabajadorApi/CambiarEstado",
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

        public async Task<IToReturn<Trabajadores>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, Trabajadores>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/TrabajadorApi/GetById",
                    id
                );

                return new ToReturn<Trabajadores>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Trabajadores>(ex.Message),
                    _ => new ToReturnError<Trabajadores>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<int>> InsertAsync(Trabajadores trabajadores)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Trabajadores, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/TrabajadorApi/Insert",
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

        public async Task<IToReturn<int>> UpdateAsync(int id, Trabajadores trabajadores)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Trabajadores, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/TrabajadorApi/Update/{id}",
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

            return 500;
        }
    }
}
