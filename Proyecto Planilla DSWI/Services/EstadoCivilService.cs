using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class EstadoCivilService : IEstadoCivilService
    {
        private readonly string _apiBaseUrl;

        public EstadoCivilService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(EstadosCiviles estadoCivil)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<EstadosCiviles, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/EstadosCivilesApi/Insert",
                    estadoCivil
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

        public async Task<IToReturn<int>> UpdateAsync(int id, EstadosCiviles estadoCivil)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<EstadosCiviles, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/EstadosCivilesApi/Update/{id}",
                    estadoCivil
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
                    "/api/EstadosCivilesApi/CambiarEstado",
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

        public async Task<IToReturnList<EstadosCiviles>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<EstadosCiviles>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/EstadosCivilesApi",
                    $"Busqueda?estado={estado}"
                );

                return new ToReturnList<EstadosCiviles>(result ?? new List<EstadosCiviles>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<EstadosCiviles>(ex.Message),
                    _ => new ToReturnErrorList<EstadosCiviles>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Paginacion<List<EstadosCiviles>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<EstadosCiviles>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/EstadosCivilesApi/BusquedaPaginada",
                   new BusquedaPaginacion { page = page, pageSize = pageSize, estado = estado }
                );

                return new ToReturn<Paginacion<List<EstadosCiviles>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<EstadosCiviles>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<EstadosCiviles>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<EstadosCiviles>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, EstadosCiviles>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/EstadosCivilesApi/GetById",
                    id
                );

                return new ToReturn<EstadosCiviles>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<EstadosCiviles>(ex.Message),
                    _ => new ToReturnError<EstadosCiviles>(ex.Message)
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