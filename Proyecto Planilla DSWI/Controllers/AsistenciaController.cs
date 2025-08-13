using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using System.ComponentModel;
using System.Linq;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly AsistenciaTrabajadorLog _asistenciaLog;

        public AsistenciaController()
        {
            _asistenciaLog = new AsistenciaTrabajadorLog();
        }

        public IActionResult CargaAsistencia(List<AsistenciaTrabajadorResponse> datos)
        {
            ViewBag.Meses = GlobalVariables.Mes();
            ViewBag.Años = GlobalVariables.AñoPeriodo();

            // Establecer valores por defecto
            ViewBag.MesSeleccionado = DateTime.Now.Month;
            ViewBag.AñoSeleccionado = DateTime.Now.Year;

            return View(datos!=null && datos.Any()?datos: new List<AsistenciaTrabajadorResponse>());
        }

        [HttpPost]
        public IActionResult BuscarAsistencia(int año, int mes)
        {
            try
            {
                var resultado = _asistenciaLog.BuscarAsistenciaByPeriodo(año, mes);
                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;

                return View("CargaAsistencia", resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public IActionResult DescargarExcel(int año, int mes, List<AsistenciaTrabajadorResponse> datos   )
        {
            try
            {
               // ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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
                    if (datos!=null && datos.Any())
                    {
                    // Datos
                    int row = 2;
                    foreach (var item in datos)
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
                string excelName = $"Asistencia_{año}_{mes}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public IActionResult CargarExcel(int año, int mes, IFormFile archivoExcel, List<AsistenciaTrabajadorResponse> datosActuales)
        {
            try
            {
                if (archivoExcel == null || archivoExcel.Length == 0)
                {
                    TempData["Error"] = "Por favor seleccione un archivo Excel";
                    return RedirectToAction("CargaAsistencia");
                }

                var listaActualizada = new List<AsistenciaTrabajadorResponse>();
                datosActuales = _asistenciaLog.BuscarAsistenciaByPeriodo(año, mes);
                var trabajadoresExistentes = datosActuales.Select(d => d.Documento.Trim()).ToList();
          

              //  ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {
                    archivoExcel.CopyTo(stream);
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
listaActualizada.AddRange(datosActuales.Where(d => !listaActualizada.Any(l => l.Documento.Trim() == d.Documento.Trim())));
                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;
                ViewBag.Mensaje = "Archivo Excel cargado correctamente";

                return View("CargaAsistencia", listaActualizada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public IActionResult GrabarAsistencia(int año, int mes, List<AsistenciasTrabajadores> datos)
        {
            try
            {
                if (datos == null || !datos.Any())
                {
                    TempData["Error"] = "No hay datos para guardar";
                    return RedirectToAction("CargaAsistencia");
                }

                var resultado = _asistenciaLog.InsertarLista(datos);

                if (resultado)
                {
                    TempData["Success"] = "Datos guardados correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo guardar los datos";
                }

                return RedirectToAction("CargaAsistencia");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }
    }
}

