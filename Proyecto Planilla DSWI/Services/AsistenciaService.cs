using Newtonsoft.Json;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly string _apiBaseUrl;

        public AsistenciaService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }

        public async Task<IToReturnList<AsistenciaTrabajadorResponse>> BuscarAsistenciaByPeriodoAsync(int año, int mes)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<AsistenciaTrabajadorResponse>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    $"/api/AsistenciaApi",
                    $"BuscarAsistenciaByPeriodo?año={año}&mes={mes}"
                );

                return new ToReturnList<AsistenciaTrabajadorResponse>(result ?? new List<AsistenciaTrabajadorResponse>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<AsistenciaTrabajadorResponse>(ex.Message),
                    _ => new ToReturnErrorList<AsistenciaTrabajadorResponse>(ex.Message)
                };
            }
        }

        public async Task<IToReturn<ArchivoResponse>> DescargarExcelAsync(int año, int mes, List<AsistenciaTrabajadorResponse> datos)
        {
            try
            {
                var request = new
                {
                    Año = año,
                    Mes = mes,
                    Datos = datos
                };

                var result = await ToResponse.HTTPExecuteAsync<object, DescargarExcelResponse>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/AsistenciaApi/DescargarExcel",
                    request
                );

                var archivoResponse = new ArchivoResponse
                {
                    FileName = result.FileName,
                    FileContent = Convert.FromBase64String(result.FileContent.ToString()),
                    ContentType = result.ContentType
                };

                return new ToReturn<ArchivoResponse>(archivoResponse);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontrado<ArchivoResponse>(ex.Message),
                    _ => new ToReturnError<ArchivoResponse>(ex.Message)
                };
            }
        }

        public async Task<IToReturnList<AsistenciaTrabajadorResponse>> ProcesarExcelAsync(int año, int mes, IFormFile archivoExcel)
        {
            try
            {
                using var client = new HttpClient();
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(año.ToString()), "Año");
                content.Add(new StringContent(mes.ToString()), "Mes");

                if (archivoExcel != null)
                {
                    var fileContent = new StreamContent(archivoExcel.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivoExcel.ContentType);
                    content.Add(fileContent, "ArchivoExcel", archivoExcel.FileName);
                }

                var response = await client.PostAsync($"{_apiBaseUrl}/api/AsistenciaApi/ProcesarExcel", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    var datos = JsonConvert.DeserializeObject<List<AsistenciaTrabajadorResponse>>(apiResponse.data.ToString());
                    return new ToReturnList<AsistenciaTrabajadorResponse>(datos);
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    var statusCode = (int)response.StatusCode;

                    return statusCode switch
                    {
                        412 => new ToReturnNoEncontradoList<AsistenciaTrabajadorResponse>(errorResponse.message?.ToString() ?? "Error de validación"),
                        404 => new ToReturnNoEncontradoList<AsistenciaTrabajadorResponse>(errorResponse.message?.ToString() ?? "No encontrado"),
                        _ => new ToReturnErrorList<AsistenciaTrabajadorResponse>(errorResponse.message?.ToString() ?? "Error del servidor")
                    };
                }
            }
            catch (Exception ex)
            {
                return new ToReturnErrorList<AsistenciaTrabajadorResponse>(ex.Message);
            }
        }

        public async Task<IToReturn<bool>> GrabarAsistenciaAsync(GrabarAsistencias datos)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<GrabarAsistencias, bool>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/api/AsistenciaApi/GrabarAsistencia",
                    datos
                );

                return new ToReturn<bool>(result);
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    412 => new ToReturnValidation<bool>(ex.Message),
                    404 => new ToReturnNoEncontrado<bool>(ex.Message),
                    _ => new ToReturnError<bool>(ex.Message)
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
        public class GrabarAsistencias
        {
            public List<AsistenciasTrabajadores> Datos { get; set; }
            public int Año { get; set; }
            public int Mes { get; set; }
        }
    }
}
