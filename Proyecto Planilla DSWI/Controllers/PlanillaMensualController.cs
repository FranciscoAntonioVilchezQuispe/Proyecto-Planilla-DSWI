using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Proyecto_Planilla_DSWI.Data;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Proyecto_Planilla_Utils.GlobalEnum;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class PlanillaMensualController : Controller
    {
        private readonly PlanillaMensualLog _planillaLog;
        private readonly TrabajadorLog _trabajadorLog;

        public PlanillaMensualController()
        {
            _planillaLog = new PlanillaMensualLog();
            _trabajadorLog = new TrabajadorLog();
        }

        public IActionResult Index()
        {
            ViewBag.Meses = GlobalVariables.Mes();
            ViewBag.Años = GlobalVariables.AñoPeriodo();

            // Establecer valores por defecto
            ViewBag.MesSeleccionado = DateTime.Now.Month;
            ViewBag.AñoSeleccionado = DateTime.Now.Year;
           

            return View(); 
        }

        [HttpPost]
        public IActionResult BuscarPlanilla(int año, int mes)
        {
            try
            {
                var resultado = _planillaLog.Lista(año, mes);
                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;
                return View("Index", resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult CalcularPlanilla(int año, int mes)
        {
            try
            {
                var resultado = _planillaLog.CalcularPlanillaByPeriodo(año, mes);
            
                
                TempData["Success"] = "Planilla calculada correctamente";
                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;
                return View("Index", resultado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult GrabarPlanilla(int año, int mes, List<PlanillaMensual> datos)
        {
            try

            {
                if (datos == null || !datos.Any())
                {
                    TempData["Error"] = "No hay datos para guardar";
                    return RedirectToAction(nameof(Index));
                }
                var resultado = _planillaLog.CalcularPlanillaByPeriodo(año, mes);
                var resultadodos = _planillaLog.InsertarLista(datos);
                TempData["Success"] = resultadodos
                    ? "Planilla guardada correctamente"
                    : "No se pudo guardar la planilla";
                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet] 
        public IActionResult DescargarExcel(int año, int mes)
        {
            try
            {
         
                List<PlanillaMensual> datos = _planillaLog.Lista(año, mes);

                if (datos == null || !datos.Any())
                {
                    TempData["Error"] = "No hay datos para exportar";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Generar el Excel
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("PlanillaMensual");


                    worksheet.Cells[1, 1].Value = "Trabajador";
                    worksheet.Cells[1, 2].Value = "Cargo";
                    worksheet.Cells[1, 3].Value = "Días Trabajados";
                    worksheet.Cells[1, 4].Value = "Horas Extras (25%)";
                    worksheet.Cells[1, 5].Value = "Horas Extras (35%)";
                    worksheet.Cells[1, 6].Value = "Total Ingresos";
                    worksheet.Cells[1, 7].Value = "Total Descuentos";
                    worksheet.Cells[1, 8].Value = "Neto a Pagar";


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

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"PlanillaMensual_{año}_{mes}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar Excel: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpGet]
        public IActionResult GenerarBoleta(int idTrabajador, int año, int mes)
        {
            try
            {
                var trabajador = _trabajadorLog.Busqueda(new BuquedaTrabajador { Busqueda = "", Estado = _Estado.Activo })
                    .FirstOrDefault(t => t.IdTrabajador == idTrabajador);

                if (trabajador == null)
                {
                    TempData["Error"] = "Trabajador no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Generar el HTML de la boleta
                string boletaHtml = _planillaLog.BuscarBoleta(new BusquedaBoleta
                {
                    Año = año,
                    Mes = mes,
                    Documento = trabajador.Documento
                });


                ViewBag.BoletaHtml = boletaHtml;
                ViewBag.NombreCompleto = $"{trabajador.Nombres} {trabajador.ApellidoPaterno} {trabajador.ApellidoMaterno}";
                ViewBag.Año = año;
                ViewBag.Mes = mes;

                return View(); // Vista sin modelo fuerte
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}