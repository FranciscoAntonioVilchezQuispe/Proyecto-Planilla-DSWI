using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class SituacionTrabajadorService : ISituacionTrabajadorService
    {
        private readonly string _apiBaseUrl;

        public SituacionTrabajadorService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(SituacionTrabajador situacionTrabajador)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<SituacionTrabajador, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/SituacionTrabajadorApi/Insert",
                    situacionTrabajador
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

        public async Task<IToReturn<int>> UpdateAsync(int id, SituacionTrabajador situacionTrabajador)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<SituacionTrabajador, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/SituacionTrabajadorApi/Update/{id}",
                    situacionTrabajador
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
                    "/api/SituacionTrabajadorApi/CambiarEstado",
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

        public async Task<IToReturnList<SituacionTrabajador>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<SituacionTrabajador>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/SituacionTrabajadorApi/Busqueda?estado={estado}",
                    null
                );

                return new ToReturnList<SituacionTrabajador>(result ?? new List<SituacionTrabajador>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<SituacionTrabajador>(ex.Message),
                    _ => new ToReturnErrorList<SituacionTrabajador>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Paginacion<List<SituacionTrabajador>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<SituacionTrabajador>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/SituacionTrabajadorApi/BusquedaPaginada",
                   new BusquedaPaginacion { page = page, pageSize = pageSize, estado = estado }
                );

                return new ToReturn<Paginacion<List<SituacionTrabajador>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<SituacionTrabajador>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<SituacionTrabajador>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<SituacionTrabajador>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, SituacionTrabajador>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/SituacionTrabajadorApi/GetById",
                    id
                );

                return new ToReturn<SituacionTrabajador>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<SituacionTrabajador>(ex.Message),
                    _ => new ToReturnError<SituacionTrabajador>(ex.Message)
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
