using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;
using System.ComponentModel;
using System.Linq;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        public IActionResult CargaAsistencia(List<AsistenciaTrabajadorResponse> datos = null)
        {
            ViewBag.Meses = GlobalVariables.Mes();
            ViewBag.Años = GlobalVariables.AñoPeriodo();

            // Establecer valores por defecto
            ViewBag.MesSeleccionado = DateTime.Now.Month;
            ViewBag.AñoSeleccionado = DateTime.Now.Year;

            return View(datos != null && datos.Any() ? datos : new List<AsistenciaTrabajadorResponse>());
        }

        [HttpPost]
        public async Task<IActionResult> BuscarAsistencia(int año, int mes)
        {
            try
            {
                var resultado = await _asistenciaService.BuscarAsistenciaByPeriodoAsync(año, mes);

                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;

                if (resultado.Status == 200)
                {
                    return View("CargaAsistencia", resultado.Data);
                }
                else
                {
                    TempData["Error"] = resultado.Message;
                    return View("CargaAsistencia", new List<AsistenciaTrabajadorResponse>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DescargarExcel(int año, int mes, List<AsistenciaTrabajadorResponse> datos)
        {
            try
            {
                var resultado = await _asistenciaService.DescargarExcelAsync(año, mes, datos);

                if (resultado.Status == 200)
                {
                    var archivo = resultado.Data;
                    return File(archivo.FileContent, archivo.ContentType, archivo.FileName);
                }
                else
                {
                    TempData["Error"] = resultado.Message;
                    return RedirectToAction("CargaAsistencia");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CargarExcel(int año, int mes, IFormFile archivoExcel, List<AsistenciaTrabajadorResponse> datosActuales)
        {
            try
            {
                if (archivoExcel == null || archivoExcel.Length == 0)
                {
                    TempData["Error"] = "Por favor seleccione un archivo Excel";
                    return RedirectToAction("CargaAsistencia");
                }

                var resultado = await _asistenciaService.ProcesarExcelAsync(año, mes, archivoExcel);

                ViewBag.Meses = GlobalVariables.Mes();
                ViewBag.Años = GlobalVariables.AñoPeriodo();
                ViewBag.MesSeleccionado = mes;
                ViewBag.AñoSeleccionado = año;

                if (resultado.Status == 200)
                {
                    ViewBag.Mensaje = "Archivo Excel cargado correctamente";
                    return View("CargaAsistencia", resultado.Data);
                }
                else
                {
                    TempData["Error"] = resultado.Message;
                    return RedirectToAction("CargaAsistencia");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CargaAsistencia");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GrabarAsistencia(int año, int mes, List<AsistenciasTrabajadores> datos)
        {
            try
            {
                if (datos == null || !datos.Any())
                {
                    TempData["Error"] = "No hay datos para guardar";
                    return RedirectToAction("CargaAsistencia");
                }

                var resultado = await _asistenciaService.GrabarAsistenciaAsync(new Services.AsistenciaService.GrabarAsistencias {Año=año,Mes=mes,Datos=datos });

                if (resultado.Status == 200 && resultado.Data)
                {
                    TempData["Success"] = "Datos guardados correctamente";
                }
                else
                {
                    TempData["Error"] = resultado.Message ?? "No se pudo guardar los datos";
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

