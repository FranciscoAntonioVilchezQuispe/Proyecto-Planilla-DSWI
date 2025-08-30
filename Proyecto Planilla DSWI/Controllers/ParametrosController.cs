using Microsoft.AspNetCore.Mvc;
using Proyecto_Planilla_DSWI.Interfaces;
using Proyecto_Planilla_Entidades;
using Proyecto_Planilla_Utils;

namespace Proyecto_Planilla_DSWI.Controllers
{
    public class ParametrosController : Controller
    {
        private readonly IParametroService _parametroService;

        public ParametrosController(IParametroService parametroService)
        {
            _parametroService = parametroService;
        }

        public async Task<IActionResult> Manage()
        {
            var result = await _parametroService.BusquedaOneAsync();

            if (result.Status == 200)
            {
                return View(result.Data ?? new Parametros());
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new Parametros());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(Parametros parametros)
        {
            if (ModelState.IsValid)
            {
                IToReturn<int> result;
                if (parametros.IdParametro == 0)
                {
                    result = await _parametroService.InsertAsync(parametros);
                    TempData["SuccessMessage"] = "Parámetros creados exitosamente.";
                }
                else
                {
                    result = await _parametroService.UpdateAsync(parametros.IdParametro, parametros);
                    TempData["SuccessMessage"] = "Parámetros actualizados exitosamente.";
                }

                if (result.Status == 200)
                {
                    return RedirectToAction(nameof(Manage));
                }
                else
                {
                    if (result.Status == 412)
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result.Message;
                    }
                }
            }

            return View(parametros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado()
        {
            var parametro = await _parametroService.BusquedaOneAsync();
            if (parametro.Status != 200 || parametro.Data == null)
            {
                TempData["ErrorMessage"] = "No existe registro de parámetros para cambiar estado";
                return RedirectToAction(nameof(Manage));
            }

            var result = await _parametroService.CambiarEstadoAsync(parametro.Data.IdParametro);

            if (result.Status == 200)
            {
                TempData["SuccessMessage"] = "Estado de los parámetros cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
