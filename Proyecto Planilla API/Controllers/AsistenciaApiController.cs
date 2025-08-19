using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Proyecto_Planilla_API.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using Proyecto_Planilla_Utils.Response;

namespace Proyecto_Planilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class AsistenciaApiController : ControllerBase
        {
            private readonly AsistenciaTrabajadorLog _asistenciaLog;

            public AsistenciaApiController()
            {
                _asistenciaLog = new AsistenciaTrabajadorLog();
            }

            [HttpGet("BuscarAsistenciaByPeriodo")]
            public IActionResult BuscarAsistenciaByPeriodo(int año, int mes)
            {
                try
                {
                    var resultado = _asistenciaLog.BuscarAsistenciaByPeriodo(año, mes);

                    if (resultado != null && resultado.Any())
                    {
                        var respuesta = new ToReturnList<AsistenciaTrabajadorResponse>(resultado);
                        return StatusCode(respuesta.Status, respuesta);
                    }
                    else
                    {
                        var noEncontrado = new ToReturnNoEncontradoList<AsistenciaTrabajadorResponse>("No se encontraron datos de asistencia para el período especificado");
                        return StatusCode(noEncontrado.Status, noEncontrado);
                    }
                }
                catch (Exception ex)
                {
                    var error = new ToReturnErrorList<AsistenciaTrabajadorResponse>($"{ex.Message} {ex.InnerException}");
                    return StatusCode(error.Status, error);
                }
            }

            [HttpPost("DescargarExcel")]
            public IActionResult DescargarExcel([FromBody] DescargarExcelRequest request)
            {
                try
                {
                    var stream = new MemoryStream();

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Asistencia");

                        // Encabezados
                        worksheet.Cells[1, 1].Value = "Dni";
                        worksheet.Cells[1, 2].Value = "DiasLaborales";
                        worksheet.Cells[1, 3].Value = "DiasDescanso";
                        worksheet.Cells[1, 4].Value = "DiasInasistencia";
                        worksheet.Cells[1, 5].Value = "DiasFeriados";
                        worksheet.Cells[1, 6].Value = "HorasExtra25";
                        worksheet.Cells[1, 7].Value = "HorasExtra35";

                        if (request.Datos != null && request.Datos.Any())
                        {
                            // Datos
                            int row = 2;
                            foreach (var item in request.Datos)
                            {
                                worksheet.Cells[row, 1].Value = item.Documento;
                                worksheet.Cells[row, 2].Value = item.DiasLaborales;
                                worksheet.Cells[row, 3].Value = item.DiasDescanso;
                                worksheet.Cells[row, 4].Value = item.DiasInasistencia;
                                worksheet.Cells[row, 5].Value = item.DiasFeriados;
                                worksheet.Cells[row, 6].Value = item.HorasExtra25;
                                worksheet.Cells[row, 7].Value = item.HorasExtra35;
                                row++;
                            }
                        }

                        package.Save();
                    }

                    stream.Position = 0;
                    var fileBytes = stream.ToArray();
                    var fileName = $"Asistencia_{request.Año}_{request.Mes}.xlsx";

                    var response = new
                    {
                        FileName = fileName,
                        FileContent = Convert.ToBase64String(fileBytes),
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    };

                    var respuesta = new ToReturn<object>(response);
                    return StatusCode(respuesta.Status, respuesta);
                }
                catch (Exception ex)
                {
                    var error = new ToReturnError<object>($"{ex.Message} {ex.InnerException}");
                    return StatusCode(error.Status, error);
                }
            }

            [HttpPost("ProcesarExcel")]
            public IActionResult ProcesarExcel([FromForm] ProcesarExcelRequest request)
            {
                try
                {
                    if (request.ArchivoExcel == null || request.ArchivoExcel.Length == 0)
                    {
                        var validationError = new ToReturnValidation<List<AsistenciaTrabajadorResponse>>("Por favor seleccione un archivo Excel válido");
                        return StatusCode(validationError.Status, validationError);
                    }

                    var listaActualizada = new List<AsistenciaTrabajadorResponse>();
                    var datosActuales = _asistenciaLog.BuscarAsistenciaByPeriodo(request.Año, request.Mes);
                    var trabajadoresExistentes = datosActuales.Select(d => d.Documento.Trim()).ToList();

                    using (var stream = new MemoryStream())
                    {
                        request.ArchivoExcel.CopyTo(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.Rows;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                string dni = worksheet.Cells[row, 1].Value?.ToString()?.Trim();

                                if (!string.IsNullOrEmpty(dni) && trabajadoresExistentes.Contains(dni))
                                {
                                    var itemActual = datosActuales.FirstOrDefault(d => d.Documento.Trim() == dni);
                                    if (itemActual != null)
                                    {
                                        var itemActualizado = new AsistenciaTrabajadorResponse
                                        {
                                            IdTrabajador = itemActual.IdTrabajador,
                                            Documento = itemActual.Documento,
                                            Nombre = itemActual.Nombre,
                                            DiasLaborales = int.Parse(worksheet.Cells[row, 2].Value?.ToString() ?? "0"),
                                            DiasDescanso = int.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                                            DiasInasistencia = int.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0"),
                                            DiasFeriados = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                                            HorasExtra25 = decimal.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                                            HorasExtra35 = decimal.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0")
                                        };

                                        listaActualizada.Add(itemActualizado);
                                    }
                                }
                            }
                        }
                    }

                    // Agregar los datos que no fueron actualizados
                    listaActualizada.AddRange(datosActuales.Where(d => !listaActualizada.Any(l => l.Documento.Trim() == d.Documento.Trim())));

                    var respuesta = new ToReturnList<AsistenciaTrabajadorResponse>(listaActualizada);
                    return StatusCode(respuesta.Status, respuesta);
                }
                catch (Exception ex)
                {
                    var error = new ToReturnErrorList<AsistenciaTrabajadorResponse>($"{ex.Message} {ex.InnerException}");
                    return StatusCode(error.Status, error);
                }
            }

            [HttpPost("GrabarAsistencia")]
            public IActionResult GrabarAsistencia([FromBody] AsistenciaTrabajadorLog.GrabarAsistencias datos)
            {
                try
                {
                    if (datos == null || !datos.Datos.Any())
                    {
                        var validationError = new ToReturnValidation<bool>("No hay datos para guardar");
                        return StatusCode(validationError.Status, validationError);
                    }

                    var resultado = _asistenciaLog.InsertarLista(datos);

                    if (resultado)
                    {
                        var respuesta = new ToReturn<bool>(resultado);
                        return StatusCode(respuesta.Status, respuesta);
                    }
                    else
                    {
                        var error = new ToReturnError<bool>("No se pudo guardar los datos");
                        return StatusCode(error.Status, error);
                    }
                }
                catch (Exception ex)
                {
                    var error = new ToReturnError<bool>($"{ex.Message} {ex.InnerException}");
                    return StatusCode(error.Status, error);
                }
            }
        
    }
}
