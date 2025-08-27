using Humanizer.DateTimeHumanizeStrategy;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;
using Proyecto_Planilla_Utils.Response;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class PlanillaMensualController : Controller
    {
        private readonly IPlanillaMensualService _planillaMensualService;
        public IActionResult Index()

        {
            return View();
        }

        public PlanillaMensualController(IPlanillaMensualService planillaMensualService)
        {
            _planillaMensualService = planillaMensualService;
        }


    
       [HttpGet]
        public async Task<IActionResult> CalcularPlanilla(int año, int mes)
        {
            var result = await _planillaMensualService.CalcularPlanillaByPeriodoAsync(año, mes);

            if (result.Status == 200)
            {
                ViewBag.AñoSeleccionado = año;
                ViewBag.MesSeleccionado = mes;
                return View("Index", result.Data ?? new List<PlanillaMensual>());
            }
            else
            {
                TempData["Error"] = result.Message;
                return View("Index", new List<PlanillaMensual>());
                //return RedirectToAction(nameof(BuscarPlanilla), new { año, mes });
            }
        }
      

        [HttpGet]
        public async Task<IActionResult> BuscarPlanilla(int año, int mes, string operacion)
        {
            

            if (operacion == "calcular")
            {
                return await CalcularPlanilla(año, mes);
            }

            var result = await _planillaMensualService.ListaAsync(año, mes);
            if (result.Status == 200)
            {

                ViewBag.AñoSeleccionado = año;
                ViewBag.MesSeleccionado = mes;
                return View("Index", result.Data ?? new List<PlanillaMensual>());
            }
            else
            {
                TempData["Error"] = result.Message;
                return View("Index", new List<PlanillaMensual>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrabarPlanilla(List<PlanillaMensual> datos, int año, int mes)
        {
            var result = await _planillaMensualService.GrabarPlanillaAsync(datos);
            foreach(var item in datos)
            {
                Console.WriteLine($"desde controller {item.Nombre}");
                Console.WriteLine($"desde controller {item.Año}");
                Console.WriteLine($"desde controller {item.Mes}");
            }

            if (result.Status == 200)
            {
                TempData["Success"] = "Planilla grabada correctamente.";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(BuscarPlanilla), new { año, mes });
        }


        [HttpGet]
        public async Task<IActionResult> GenerarBoleta(int idTrabajador, int año, int mes)
        {
            var html = await _planillaMensualService.GenerarBoletaAsync(idTrabajador, año, mes);

            return Content(html, "text/html");
        }


        [HttpGet]
        public async Task<IActionResult> DescargarExcel(int año, int mes)
        {
            var result = await _planillaMensualService.DescargarExcelAsync(año, mes);

            if (result.Status == 200 && result.Data != null)
            {
                var archivo = result.Data;
                return File(archivo.FileContent, archivo.ContentType, archivo.FileName);
            }
            else
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(BuscarPlanilla), new { año, mes });
            }
        }
    }
}
