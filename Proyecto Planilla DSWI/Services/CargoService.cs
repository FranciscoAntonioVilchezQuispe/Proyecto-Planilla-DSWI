using Newtonsoft.Json;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using System.Text;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class CargoService:ICargoService
    {
        private readonly string _apiBaseUrl;

        public CargoService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(Cargos cargo)
        {
            try
            {
                // Usar tu clase ToResponse para hacer la llamada
                var result = await ToResponse.HTTPExecuteAsync<Cargos, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/CargosApi/Insert",
                    cargo
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

        public async Task<IToReturn<int>> UpdateAsync(int id, Cargos cargo)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Cargos, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/CargosApi/Update/{id}",
                    cargo
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
                    "/api/CargosApi/CambiarEstado",
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

        public async Task<IToReturnList<Cargos>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<Cargos>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/CargosApi",
                    $"Busqueda?estado={estado}"
                );

                return new ToReturnList<Cargos>(result ?? new List<Cargos>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<Cargos>(ex.Message),
                    _ => new ToReturnErrorList<Cargos>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Paginacion<List<Cargos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 2, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<Cargos>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/CargosApi/BusquedaPaginada",
                   new BusquedaPaginacion { page=page,pageSize=pageSize,estado=estado}
                );

                return new ToReturn<Paginacion<List<Cargos>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<Cargos>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<Cargos>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Cargos>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, Cargos>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/CargosApi/GetById",
                    id
                );

                return new ToReturn<Cargos>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Cargos>(ex.Message),
                    _ => new ToReturnError<Cargos>(ex.Message)
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
