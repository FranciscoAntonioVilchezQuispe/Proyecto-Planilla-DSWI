using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class GeneroService : IGeneroService
    {
        private readonly string _apiBaseUrl;

        public GeneroService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturn<int>> InsertAsync(Generos genero)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Generos, int>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/GenerosApi/Insert",
                    genero
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

        public async Task<IToReturn<int>> UpdateAsync(int id, Generos genero)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<Generos, int>(
                    Metodo.PUT,
                    _apiBaseUrl,
                    $"/api/GenerosApi/Update/{id}",
                    genero
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
                    "/api/GenerosApi/CambiarEstado",
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

        public async Task<IToReturnList<Generos>> BusquedaAsync(_Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<Generos>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/GenerosApi/Busqueda?estado={estado}",
                    null
                );

                return new ToReturnList<Generos>(result ?? new List<Generos>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<Generos>(ex.Message),
                    _ => new ToReturnErrorList<Generos>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Paginacion<List<Generos>>>> BusquedaPaginadaAsync(int page = 1, int pageSize = 10, _Estado estado = _Estado.Todos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<BusquedaPaginacion, Paginacion<List<Generos>>>(
                    Metodo.POST,
                    _apiBaseUrl,
                    $"/api/GenerosApi/BusquedaPaginada",
                   new BusquedaPaginacion { page = page, pageSize = pageSize, estado = estado }
                );

                return new ToReturn<Paginacion<List<Generos>>>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Paginacion<List<Generos>>>(ex.Message),
                    _ => new ToReturnError<Paginacion<List<Generos>>>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<Generos>> GetByIdAsync(int id)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<int, Generos>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/api/GenerosApi/GetById",
                    id
                );

                return new ToReturn<Generos>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<Generos>(ex.Message),
                    _ => new ToReturnError<Generos>(ex.Message)
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
