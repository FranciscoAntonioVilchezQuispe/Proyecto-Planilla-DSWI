using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Services
{
    public class PlanillaMensualService : IPlanillaMensualService
    {
        private readonly string _apiBaseUrl;

        public PlanillaMensualService(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiService:URL"];
        }



        public async Task<IToReturnList<PlanillaMensual>> CalcularPlanillaByPeriodoAsync(int año, int mes)
        {
            try
            {
                
                var result = await ToResponse.HTTPExecuteAsync<string, List<PlanillaMensual>>(
                    Metodo.GET,
                    _apiBaseUrl,
                   $"/CalcularPlanillaByPeriodo",
                   $"?año={año}&mes={mes}"


                );

                

                return new ToReturnList<PlanillaMensual>(result ?? new List<PlanillaMensual>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);
                

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<PlanillaMensual>(ex.Message),
                    _ => new ToReturnErrorList<PlanillaMensual>(ex.Message)
                };
            }
        }


        public async Task<IToReturn<bool>> GrabarPlanillaAsync(List<PlanillaMensual> datos)
        {
            try
            {
                foreach (var item in datos)
                {
                    Console.WriteLine(item.Nombre);
                    Console.WriteLine(item.Año);
                    Console.WriteLine(item.Mes);
                }
                var result = await ToResponse.HTTPExecuteAsync<List<PlanillaMensual>, bool>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/GrabarPlanilla",
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

        public async Task<IToReturn<ArchivoResponse>> DescargarExcelAsync(int año, int mes)
        {
            try
            {
                var request = new
                {
                    Año = año,
                    Mes = mes
                };

                var result = await ToResponse.HTTPExecuteAsync<object, DescargarExcelResponse>(
                    Metodo.POST,
                    _apiBaseUrl,
                    "/DescargarExcelPlanilla",
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


        public async Task<IToReturnList<PlanillaMensual>> ListaAsync(int año, int mes)
        {
            try
            {
                var result = await ToResponse.HTTPExecuteAsync<string, List<PlanillaMensual>>(
                    Metodo.GET,
                    _apiBaseUrl,
                    "/Lista",
                    $"?año={año}&mes={mes}"
                );

                return new ToReturnList<PlanillaMensual>(result ?? new List<PlanillaMensual>());
            }
            catch (Exception ex)
            {
                var statusCode = ExtractStatusCodeFromException(ex);

                return statusCode switch
                {
                    404 => new ToReturnNoEncontradoList<PlanillaMensual>(ex.Message),
                    _ => new ToReturnErrorList<PlanillaMensual>(ex.Message)
                };
            }
        }

        /*public async Task<string> GenerarBoletaAsync(int idTrabajador, int año, int mes)
        {
            try
            {
                // Para GET, el objeto se concatena en la URL, así que pasamos null como data
                var result = await ToResponse.HTTPExecuteAsync<string, object>(
                    GlobalEnum.Metodo.GET,
                    _apiBaseUrl,
                    "/GenerarBoleta",
                    $"?idTrabajador={idTrabajador}&año={año}&mes={mes}"
                    //null
                );

                /*"/GenerarBoleta",
                    $"?idTrabajador={idTrabajador}&año={año}&mes={mes}"

                return (string)(result ?? "<h3>Error: Respuesta vacía</h3>");
            }
            catch (Exception ex)
            {
                return $"<h3>Error al generar boleta: {ex.Message}</h3>";
            }
        }*/
        public async Task<string> GenerarBoletaAsync(int idTrabajador, int año, int mes)
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(_apiBaseUrl);

                var response = await client.GetAsync($"/GenerarBoleta?idTrabajador={idTrabajador}&año={año}&mes={mes}");
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                return html;
            }
            catch (Exception ex)
            {
                return $"<h3>Error al generar boleta: {ex.Message}</h3>";
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
