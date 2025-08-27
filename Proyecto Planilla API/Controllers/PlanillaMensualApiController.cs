using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Proyecto_Planilla_API.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto_Planilla_API.Controllers

{
    [ApiController]
    public class PlanillaMensualApiController : ControllerBase
    {
        private readonly PlanillaMensualLog _planillaLog;

        public PlanillaMensualApiController()
        {
            _planillaLog = new PlanillaMensualLog();
        }

        [HttpGet("CalcularPlanillaByPeriodo")]
        public IActionResult CalcularPlanillaByPeriodo(int año, int mes)
        {
            try
            {
                var resultado = _planillaLog.CalcularPlanillaByPeriodo(año, mes);

                if (resultado != null && resultado.Any())
                {
                    var respuesta = new ToReturnList<PlanillaMensual>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<PlanillaMensual>("No se encontraron registros de planilla para el período especificado");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<PlanillaMensual>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("GrabarPlanilla")]
        public IActionResult GrabarPlanilla([FromBody] List<PlanillaMensual> datos)
        {
            try
            {
                if (datos == null || !datos.Any())
                {
                    var validationError = new ToReturnValidation<bool>("No hay datos para guardar");
                    return StatusCode(validationError.Status, validationError);
                }

                var resultado = _planillaLog.InsertarLista(datos);

                if (resultado)
                {
                    var respuesta = new ToReturn<bool>(true);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var error = new ToReturnError<bool>("No se pudo guardar la planilla");
                    return StatusCode(error.Status, error);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnError<bool>($"{ex.Message} {ex.InnerException}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpPost("DescargarExcelPlanilla")]
        public IActionResult DescargarExcelPlanilla([FromBody] (int Año, int Mes) request)
        {
            try
            {
                var datos = _planillaLog.CalcularPlanillaByPeriodo(request.Año, request.Mes);

                if (datos == null || !datos.Any())
                {
                    var noEncontrado = new ToReturnNoEncontrado<object>("No hay datos para exportar");
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }

                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("PlanillaMensual");

                    worksheet.Cells[1, 1].Value = "Trabajador";
                    worksheet.Cells[1, 2].Value = "Cargo";
                    worksheet.Cells[1, 3].Value = "Días Trabajados";
                    worksheet.Cells[1, 4].Value = "Horas Extras (25%)";
                    worksheet.Cells[1, 5].Value = "Horas Extras (35%)";
                    worksheet.Cells[1, 6].Value = "Total Ingresos";
                    worksheet.Cells[1, 7].Value = "Total Descuentos";
                 

                    int row = 2;
                    foreach (var item in datos)
                    {
                        worksheet.Cells[row, 1].Value = $"{item.Nombre} {item.Apellido}";
                        worksheet.Cells[row, 2].Value = item.IdCargo;
                        worksheet.Cells[row, 3].Value = item.nDiasTrab;
                        worksheet.Cells[row, 4].Value = item.nHorasExtra1;
                        worksheet.Cells[row, 5].Value = item.nHorasExtra2;
                        worksheet.Cells[row, 6].Value = item.TotalIngreso;
                        worksheet.Cells[row, 7].Value = item.TotalDescuento;
                        worksheet.Cells[row, 8].Value = item.TotalNetoBoleta;
                        row++;
                    }

                    package.Save();
                }

                stream.Position = 0;
                var fileBytes = stream.ToArray();
                var fileName = $"PlanillaMensual_{request.Año}_{request.Mes}.xlsx";

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
                var error = new ToReturnError<object>($"Error al generar Excel: {ex.Message}");
                return StatusCode(error.Status, error);
            }
        }

        [HttpGet("Lista")]
        public IActionResult Lista(int año, int mes)
        {
            try
            {
                var resultado = _planillaLog.Lista(año, mes);

                if (resultado != null && resultado.Any())
                {
                    var respuesta = new ToReturnList<PlanillaMensual>(resultado);
                    return StatusCode(respuesta.Status, respuesta);
                }
                else
                {
                    var noEncontrado = new ToReturnNoEncontradoList<PlanillaMensual>(
                        "No se encontraron registros de planilla para el período especificado"
                    );
                    return StatusCode(noEncontrado.Status, noEncontrado);
                }
            }
            catch (Exception ex)
            {
                var error = new ToReturnErrorList<PlanillaMensual>($"Error al obtener la lista: {ex.Message}");
                return StatusCode(error.Status, error);
            }
        }


        [HttpGet("GenerarBoleta")]
        public IActionResult GenerarBoleta(int idTrabajador, int año, int mes)
        {
            try
            {
                var boletaHtml = _planillaLog.GenerarBoletaHtml(idTrabajador, año, mes);

                if (string.IsNullOrEmpty(boletaHtml))
                    return NotFound("No se encontró la boleta");

                // devolvemos HTML como string
                return Content(boletaHtml, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
       

    }
}